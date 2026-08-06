# TicketResolver – Full Feature Walkthrough

> A .NET MVC 5 application with a 3-layer architecture: **MVC (UI)** → **DAL (Business/Data)** → **SQL Server (Stored Procedures)**

---

## Architecture Overview

```
Browser
  └─ Views (.cshtml) ─ Controller (.cs) ─ DAL (.cs) ─ SQL Stored Procedures
                              │
                         Filters / Helpers
                    (JWT Auth, Email, Logging)
```

**3 Roles:**
| Role | What they can do |
|---|---|
| **Administrator** | Everything — manage users, assign tickets, change any status |
| **Support Executive** | Work on tickets assigned to them, change status, comment |
| **Employee** | Create tickets, view own tickets, add comments |

---

## Feature 1 – Authentication (Register / Login / OTP / Logout)

### Database Tables
- `TicketResolverUser` — stores user profile (name, email, mobile, roleId, isActive)
- `TicketResolverUserCredential` — stores bcrypt password hash (1:1 with user)
- `TicketResolverOtpVerification` — stores OTP codes with purpose (`Signup`, `Login`, `ForgotPassword`) and expiry
- `TicketResolverRefreshToken` — stores hashed refresh tokens with expiry

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverUserInsert` | Creates a new user record |
| `TicketResolverUserCredentialInsert` | Saves bcrypt password hash for a user |
| `TicketResolverUserGetByEmail` | Fetches user by email for login lookup |
| `TicketResolverUserGetById` | Fetches user by UserId |
| `TicketResolverUserCredentialGetByUserId` | Fetches stored password hash |
| `TicketResolverOtpInsert` | Saves a new OTP record with expiry |
| `TicketResolverOtpInvalidatePrevious` | Marks all previous OTPs for that email+purpose as verified (invalidated) |
| `TicketResolverOtpVerify` | Checks OTP code, email, purpose — returns `IsValid + UserId` |
| `TicketResolverOtpGetLatest` | Gets the most recent OTP for a given email+purpose |
| `TicketResolverRefreshTokenInsert` | Saves a new refresh token hash |
| `TicketResolverRefreshTokenGetByTokenHash` | Fetches token record by hash |
| `TicketResolverRefreshTokenDeactivateAll` | Deactivates all tokens for a user (used on logout/login) |
| `TicketResolverRefreshTokenRevoke` | Revokes a specific token by ID |
| `TicketResolverRefreshTokenRotate` | Revokes old token and inserts a new one atomically |
| `TicketResolverRefreshTokenUpdateLastUsed` | Updates the `LastUsedDate` on a token |
| `TicketResolverUserCredentialUpdatePassword` | Updates the password hash (for forgot password) |

### Flow

**Register:**
1. User fills Register form (FirstName, LastName, Email, Mobile, Password, Role)
2. `AuthController.Register` checks if email already exists via `GetUserByEmail`
3. If unique → generates a 6-digit OTP, saves it via `InsertOtpVerification` (purpose=`Signup`)
4. Sends OTP to the email via `EmailHelper`
5. Registration data is saved in `TempData` and user is redirected to `VerifyOtp` page
6. On OTP verification → `VerifyOtp` SP validates the code
7. If valid → `InsertUser` creates the user record, `InsertUserCredential` saves bcrypt hash
8. Account is **inactive by default** — Admin must activate it

**Login:**
1. User enters Email + Password
2. `GetUserByEmail` fetches user, checks `IsActive`
3. `GetPasswordHashByUserId` fetches hash → `PasswordHelper.VerifyPassword` compares
4. On success → generates OTP, saves via `InsertOtpVerification` (purpose=`Login`), sends email
5. Redirects to `VerifyOtp` page
6. On OTP success → JWT access token + refresh token generated
7. Both stored as **HttpOnly cookies** (`jwt_token`, `refresh_token`)
8. `DeactivateAllRefreshTokens` clears old tokens, new one inserted via `InsertRefreshToken`

**Logout:**
1. `AuthController.Logout` reads JWT from cookie, parses `UserId` from claims
2. Calls `DeactivateAllRefreshTokens` to invalidate DB tokens
3. Expires both cookies → redirects to Login

---

## Feature 2 – Dashboard

### Database Tables
- All ticket/user tables (read-only for stats)

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketDashboardStats` | Returns multiple result sets: stat counts (total/new/open/etc), recent tickets list, and unassigned tickets list — filtered by role |

### Flow
1. `HomeController.Index` is called after login
2. Calls `DashboardDAL.GetStats(userId, roleId)`
3. SP returns **3 result sets**:
   - **Table[0]**: Stat counts (TotalTickets, NewTickets, OpenTickets, InProgress, Resolved, Closed, Reopened)
   - **Table[1]**: Recent 5 tickets (filtered by role — Employee sees only theirs, Support Executive sees only assigned)
   - **Table[2]**: Unassigned tickets (Admins only)
4. `HomeController` maps all 3 into a `DashboardViewModel` and sends to `Index.cshtml`
5. Dashboard also renders a **Pie Chart** (using a charting library) from the stat counts

---

## Feature 3 – Ticket List & Search

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketSearch` | Paginated + filtered + sorted ticket search with role-based visibility |

### Flow
1. `TicketController.Index` loads the Tickets page
2. Accepts filter params: `searchTerm`, `categoryId`, `priorityId`, `statusId`, sort options, page number
3. Role filtering applied in `BuildTicketSearchModel`:
   - **Employee** → only sees `CreatedBy = CurrentUserId`
   - **Support Executive** → only sees `AssignedTo = CurrentUserId`
   - **Admin** → sees all, can also filter by `IsUnassigned`
4. Results mapped into `TicketSearchViewModel` with a list of `TicketListItemViewModel`
5. AJAX post to `TicketController.Index (POST)` returns `_TicketTable` partial view for partial refresh
6. **Export to CSV** available via `TicketController.Export` — runs the same search with `pageSize=9999` and outputs a `.csv` file download

---

## Feature 4 – Create Ticket

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketGenerateNumber` | Generates next ticket number in format `TKT000001` |
| `TicketResolverTicketInsert` | Inserts a new ticket with status defaulting to `New` (StatusId=1) |
| `TicketResolverTicketAttachmentInsert` | Saves file metadata (original name + stored GUID name) |

### Flow
1. User opens `Create` form — Categories and Priorities loaded from `MasterDAL`
2. On submit → `TicketController.Create (POST)`:
   - Calls `GenerateTicketNumber` → gets e.g. `TKT000006`
   - Calls `TicketDAL.Insert` → creates ticket with `StatusId=1` (New)
   - If files attached → each file saved to `~/Uploads/` with a GUID filename
   - `AttachmentDAL.Insert` records original + stored name linked to `TicketId`
3. Success message set in `TempData`, redirects to Tickets list

---

## Feature 5 – Ticket Details

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketGetById` | Fetches full ticket with joined Category, Priority, Status, Creator, Assignee names |
| `TicketResolverTicketCommentGetByTicketId` | Gets comments — internal notes filtered by role (Employees can't see internal notes) |
| `TicketResolverTicketAttachmentGetByTicketId` | Gets all attachments for the ticket |
| `TicketResolverTicketStatusHistoryGetByTicketId` | Gets full status change history |

### Flow
1. `TicketController.Details(id)` calls `GetDetailById` → gets ticket row with all joined names
2. Also calls `CommentDAL.GetByTicketId(ticketId, userId, roleId)` → SP filters internal notes by role
3. `AttachmentDAL.GetByTicketId` fetches all files
4. `HistoryDAL.GetByTicketId` fetches the status change audit trail
5. All loaded into `TicketDetailViewModel` → rendered in `Details.cshtml`
6. The view shows ticket info, status badge, comments section, attachments list, and history timeline
7. **Role-based buttons** shown conditionally:
   - Admin/Support: can Change Status, Assign
   - Creator (Employee): can Edit, Delete (if not Closed)

---

## Feature 6 – Edit Ticket

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketUpdate` | Updates Subject, Description, CategoryId, PriorityId + ModifiedBy/Date |

### Flow
1. `TicketController.Edit(id)` — checks ownership (`ticket.CreatedBy == CurrentUserId`) or Admin role
2. Blocks edit if status is `Closed` (StatusId=6) and user is Employee
3. `TicketController.Edit (POST)` calls `TicketDAL.Update`
4. Redirects to Details page with success message

---

## Feature 7 – Ticket Assignment

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketAssign` | Updates `AssignedTo` on the ticket, changes status to `Assigned` (StatusId=2), inserts a status history record |
| `TicketResolverUserSearch` | Used to fetch all active Support Executives (RoleId=2) for the dropdown |

### Flow
1. Admin/Support Executive opens `Assign` page for a ticket
2. Dropdown populated with all active Support Executives
3. On submit → `TicketDAL.Assign(ticketId, assignedTo, assignedBy, changeReason)`
4. SP atomically: updates `AssignedTo`, sets `StatusId = 2` (Assigned), inserts a `TicketStatusHistory` record
5. After assignment → sends **email notification** to the assignee with ticket details
6. Redirects to Details page

---

## Feature 8 – Change Status

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketUpdateStatus` | Updates ticket status, sets `ResolvedDate`/`ClosedDate` if applicable, inserts status history |

### Flow
1. On ticket Details page, Status dropdown available to Admin/Support Executive
2. AJAX POST to `TicketController.ChangeStatus(ticketId, newStatusId, changeReason)`
3. Calls `TicketDAL.UpdateStatus` — SP:
   - Updates `StatusId` on ticket
   - Sets `ResolvedDate` if new status = Resolved
   - Sets `ClosedDate` if new status = Closed
   - Inserts history record with old/new status + reason + who changed it
4. After DB update → `NotifyStatusChange()` fires **async background email**:
   - Sends to ticket creator and assignee (if different)
   - Uses `HostingEnvironment.QueueBackgroundWorkItem` so it doesn't block the response

---

## Feature 9 – Comments

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketCommentInsert` | Inserts a comment — supports `IsInternalNote` flag |
| `TicketResolverTicketCommentGetByTicketId` | Returns comments filtered by role — Employees don't see internal notes |

### Flow
1. Comment box on Details page — Admins/Support Executives see **"Internal Note"** checkbox
2. AJAX POST to `TicketController.AddComment(ticketId, commentText, isInternalNote, file)`
3. `CommentDAL.Insert` saves to `TicketResolverTicketComment`
4. If file attached with comment → file saved to `~/Uploads/`, `AttachmentDAL.Insert` links it to both `TicketId` and `CommentId`
5. Comments refresh on the page without full reload

---

## Feature 10 – Attachments

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketAttachmentInsert` | Stores original filename + GUID stored filename + links to ticket/comment |
| `TicketResolverTicketAttachmentGetByTicketId` | Lists all attachments for a ticket |
| `TicketResolverTicketAttachmentGetById` | Gets single attachment (used for download) |
| `TicketResolverTicketAttachmentDelete` | Soft-deletes an attachment record |

### Flow
- **Upload**: Files saved to `~/Uploads/` with a GUID name to prevent collisions. DB stores both the original name and the stored name.
- **Download**: `TicketController.Download(id)` fetches the stored filename, maps to disk path, and returns `File(path, "application/octet-stream", originalName)` so the user sees the original filename.
- **On Create**: Multiple files supported via `HttpPostedFileBase[]`
- **On Comment**: Single file per comment

---

## Feature 11 – Status History / Audit Trail

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketStatusHistoryGetByTicketId` | Returns all status transitions for a ticket — old status, new status, reason, changed by, date |

### Flow
- Every status change (via Assign SP or UpdateStatus SP) inserts a record into `TicketResolverTicketStatusHistory`
- History loaded in `TicketController.Details` via `HistoryDAL.GetByTicketId`
- Shown as a **timeline** on the Details page — who changed what, when, with what reason

---

## Feature 12 – User Management (Agents Page)

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverUserSearch` | Paginated search of users with filters (name/email, roleId, isActive) |
| `TicketResolverUserUpdate` | Updates user profile fields |
| `TicketResolverUserSetActiveStatus` | Activates or deactivates a user account |
| `TicketResolverUserInsert` | Creates a new user (used during registration) |

### Flow
1. `UserController` (Admin only) lists all users via `AuthDAL.SearchUsers`
2. Admin can filter by role, active status, name/email
3. **Edit user**: updates name, email, mobile, role via `UpdateUser` SP
4. **Activate/Deactivate**: toggle `IsActive` via `SetActiveStatus` SP — deactivated users cannot log in

---

## Feature 13 – Master Data (Settings Page)

### Stored Procedures

| SP | What it does |
|---|---|
| `TicketResolverTicketCategoryGetAll` | Lists all active categories |
| `TicketResolverTicketCategoryInsert` | Creates a new category (unique name enforced) |
| `TicketResolverTicketCategoryUpdate` | Updates category name |
| `TicketResolverTicketCategoryDelete` | Soft-deletes a category |
| `TicketResolverTicketPriorityGetAll` | Lists priorities ordered by sequence |
| `TicketResolverTicketStatusGetAll` | Lists all statuses including `IsTerminalState` flag |
| `TicketResolverRoleGetAll` | Lists all roles |

### Flow
1. `MasterDataController` (Admin only) manages the Settings page
2. Admin can **Add / Edit / Delete ticket categories** — these populate the dropdowns on Create/Edit forms
3. Priorities and Statuses are seeded and read-only in the UI (managed via SP directly)

---

## Feature 14 – Email Notifications

No stored procedures — handled in C# using `EmailHelper`.

### When emails are sent:
| Trigger | Who gets email |
|---|---|
| Register OTP | The registering user |
| Login OTP | The logging-in user |
| Ticket Assigned | The assigned Support Executive |
| Status Changed | Ticket creator + Assignee (if different) |

- Uses an **HTML email template** (`OtpEmail.html`, `NotificationEmail.html`) with placeholder replacement
- Status-change emails fire **asynchronously** via `HostingEnvironment.QueueBackgroundWorkItem` so the HTTP response returns immediately

---

## Feature 15 – JWT Authentication & Role Authorization

### JWT Flow
- After OTP verification, `JwtHelper.GenerateAccessToken` creates a signed JWT with claims: `UserId`, `Email`, `Role`
- Stored in `HttpOnly` cookie `jwt_token`
- A refresh token (random GUID, SHA-256 hashed in DB) is stored in cookie `refresh_token`
- `[RoleAuthorize]` filter on controllers validates the JWT on every request

### `RoleAuthorize` Filter
1. Reads `jwt_token` cookie
2. Validates with `JwtHelper.ValidateToken`
3. Sets `Thread.CurrentPrincipal` and `HttpContext.User` with the claims
4. If token expired → checks `refresh_token` cookie → calls `RotateRefreshToken` SP to issue new tokens
5. If both expired → redirects to Login

---

## Feature 16 – Application Logging

### Database Table
- `TicketResolverLog` — `LogLevel`, `Source`, `Message`, `Exception`, `StackTrace`, `CreatedDate`

### Flow
- `AppLogger.Error(source, message, exception)` is called in every `catch` block throughout all controllers
- `LogDAL.Insert` saves the error to the DB
- Errors also written to file logs in the `logs/` folder via a file logger

---

## Database Schema Summary

```
TicketResolverRole
TicketResolverUser ─────────── TicketResolverUserCredential (1:1)
      │                         TicketResolverRefreshToken (1:many)
      │                         TicketResolverOtpVerification (1:many)
      │
TicketResolverTicket ─┬──── TicketResolverTicketComment ─── TicketResolverTicketAttachment
      │               └──── TicketResolverTicketAttachment (ticket-level)
      │               └──── TicketResolverTicketStatusHistory
      │
TicketResolverTicketCategory
TicketResolverTicketPriority
TicketResolverTicketStatus
TicketResolverLog
```
