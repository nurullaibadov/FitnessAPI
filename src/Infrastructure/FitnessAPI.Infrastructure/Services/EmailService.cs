using FitnessAPI.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace FitnessAPI.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration) => _configuration = configuration;

    public async Task SendEmailVerificationAsync(string email, string name, string token)
    {
        var verificationUrl = $"{_configuration["AppSettings:BaseUrl"]}/verify-email?token={token}";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #4CAF50;'>Verify Your Email</h2>
            <p>Hello {name},</p>
            <p>Thank you for registering! Please click the button below to verify your email address.</p>
            <a href='{verificationUrl}' style='background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 16px 0;'>
                Verify Email
            </a>
            <p>Or copy this link: <a href='{verificationUrl}'>{verificationUrl}</a></p>
            <p>This link expires in 24 hours.</p>
        </div>";

        await SendEmailAsync(email, "Verify Your Email - Fitness App", body);
    }

    public async Task SendPasswordResetAsync(string email, string name, string token)
    {
        var resetUrl = $"{_configuration["AppSettings:BaseUrl"]}/reset-password?token={token}";
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #FF5722;'>Reset Your Password</h2>
            <p>Hello {name},</p>
            <p>You requested to reset your password. Click the button below to proceed.</p>
            <a href='{resetUrl}' style='background-color: #FF5722; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 16px 0;'>
                Reset Password
            </a>
            <p>Or copy this link: <a href='{resetUrl}'>{resetUrl}</a></p>
            <p>This link expires in 2 hours. If you did not request this, ignore this email.</p>
        </div>";

        await SendEmailAsync(email, "Reset Your Password - Fitness App", body);
    }

    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <h2 style='color: #2196F3;'>Welcome to Fitness App! 🏋️</h2>
            <p>Hello {name},</p>
            <p>We're excited to have you on board! Start your fitness journey today.</p>
            <ul>
                <li>Create your personalized workout plans</li>
                <li>Track your progress</li>
                <li>Log your workouts</li>
                <li>Monitor body measurements</li>
            </ul>
            <p>Let's get fit together!</p>
        </div>";

        await SendEmailAsync(email, "Welcome to Fitness App! 🏋️", body);
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(emailSettings["Host"], int.Parse(emailSettings["Port"]!), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
