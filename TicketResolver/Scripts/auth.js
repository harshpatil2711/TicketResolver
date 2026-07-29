$(function () {
    $('.login-form').on('submit', function (e) {
        if (!$(this).valid()) return;
        e.preventDefault();
        var form = $(this);
        $.post(form.attr('action'), form.serialize(), function (res) {
            if (res.success) {
                window.location.href = res.redirectUrl;
            } else {
                Swal.fire({ icon: 'error', title: 'Login Failed', text: res.error || 'Invalid credentials.' });
            }
        }).fail(function () {
            Swal.fire({ icon: 'error', title: 'Error', text: 'Something went wrong. Please try again.' });
        });
    });

    $('.register-form').on('submit', function (e) {
        if (!$(this).valid()) return;
        e.preventDefault();
        var form = $(this);
        $.post(form.attr('action'), form.serialize(), function (res) {
            if (res.success) {
                Swal.fire({ icon: 'success', title: 'OTP Sent', text: 'Verify your email to complete registration.', timer: 3000, showConfirmButton: false });
                setTimeout(function () { window.location.href = res.redirectUrl; }, 1000);
            } else {
                Swal.fire({ icon: 'error', title: 'Registration Failed', text: res.error || 'Please try again.' });
            }
        }).fail(function () {
            Swal.fire({ icon: 'error', title: 'Error', text: 'Something went wrong. Please try again.' });
        });
    });

    $('.otp-form').on('submit', function (e) {
        if (!$(this).valid()) return;
        e.preventDefault();
        var form = $(this);
        $.post(form.attr('action'), form.serialize(), function (res) {
            if (res.success) {
                Swal.fire({ icon: 'success', title: 'Verified!', timer: 2000, showConfirmButton: false });
                setTimeout(function () { window.location.href = res.redirectUrl; }, 1000);
            } else {
                Swal.fire({ icon: 'error', title: 'Verification Failed', text: res.error || 'Invalid OTP.' });
            }
        }).fail(function () {
            Swal.fire({ icon: 'error', title: 'Error', text: 'Something went wrong.' });
        });
    });
});

function resendOtp(link) {
    var form = link.closest('form');
    $.post(form.action, $(form).serialize(), function (res) {
        if (res.success) {
            Swal.fire({ icon: 'success', title: 'OTP Sent', text: 'A new code has been sent to your email.', timer: 3000, showConfirmButton: false });
        } else {
            Swal.fire({ icon: 'error', title: 'Failed', text: 'Could not resend OTP.' });
        }
    }).fail(function () {
        Swal.fire({ icon: 'error', title: 'Error', text: 'Something went wrong.' });
    });
}
