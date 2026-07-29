using System;
using System.IO;
using System.Web;

namespace TicketResolver.Helpers
{
    public static class AttachmentHelper
    {
        public static string SaveFile(HttpPostedFileBase file, string uploadFolder)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            var fileName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var path = Path.Combine(uploadFolder, uniqueFileName);

            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            file.SaveAs(path);
            return uniqueFileName;
        }
    }
}
