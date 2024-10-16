using Xunit;
using PROG_POE_Part2.Controllers;
using PROG_POE_Part2.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;

public class ClaimsControllerTests
{
    [Fact]
    public void SubmitClaim_ValidClaim_ReturnsRedirectToAction()
    {
        // Arrange
        var controller = new ClaimsController();
        var validClaim = new LecturerClaimViewModel
        {
            HoursWorked = 5,
            HourlyRate = 100,
            Notes = "Test claim"
        };
        var mockFile = new FormFile(new MemoryStream(), 0, 0, "SupportingDocument", "test.pdf");

        // Act
        var result = controller.SubmitClaim(validClaim, mockFile);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ClaimConfirmation", redirectResult.ActionName);
    }

    [Fact]
    public void SubmitClaim_InvalidFileSize_ReturnsModelError()
    {
        // Arrange
        var controller = new ClaimsController();
        var validClaim = new LecturerClaimViewModel { HoursWorked = 5, HourlyRate = 100 };

        // Create a mock file larger than 2MB
        var largeFile = new FormFile(new MemoryStream(new byte[3 * 1024 * 1024]), 0, 3 * 1024 * 1024, "SupportingDocument", "largefile.pdf");

        // Act
        var result = controller.SubmitClaim(validClaim, largeFile);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("SupportingDocument"));
        Assert.Contains("File size must be less than 2 MB.", controller.ModelState["SupportingDocument"].Errors[0].ErrorMessage);
    }

    [Fact]
    public void SubmitClaim_InvalidFileType_ReturnsModelError()
    {
        // Arrange
        var controller = new ClaimsController();
        var validClaim = new LecturerClaimViewModel { HoursWorked = 5, HourlyRate = 100 };

        // Create a mock file with invalid file type
        var invalidFile = new FormFile(new MemoryStream(new byte[100]), 0, 100, "SupportingDocument", "file.exe");

        // Act
        var result = controller.SubmitClaim(validClaim, invalidFile);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey("SupportingDocument"));
        Assert.Contains("Only PDF, DOCX, and XLSX files are allowed.", controller.ModelState["SupportingDocument"].Errors[0].ErrorMessage);
    }

    [Fact]
    public void ApproveClaim_ValidClaimId_ChangesStatusToApproved()
    {
        // Arrange
        var controller = new ClaimsController();
        var claim = new Claim { ClaimId = 1, Status = "Pending" };
        ClaimsController.claimsDb.Add(claim);

        // Act
        var result = controller.ApproveClaim(1);

        // Assert
        Assert.Equal("Approved", claim.Status);
    }

    [Fact]
    public void RejectClaim_ValidClaimId_ChangesStatusToRejected()
    {
        // Arrange
        var controller = new ClaimsController();
        var claim = new Claim { ClaimId = 2, Status = "Pending" };
        ClaimsController.claimsDb.Add(claim);

        // Act
        var result = controller.RejectClaim(2);

        // Assert
        Assert.Equal("Rejected", claim.Status);
    }

    [Fact]
    public void GetProgressBarClass_StatusApproved_ReturnsSuccessClass()
    {
        // Arrange
        var controller = new ClaimsController();

        // Act
        var result = controller.GetProgressBarClass("Approved");

        // Assert
        Assert.Equal("bg-success", result);
    }

    [Fact]
    public void GetProgressPercentage_StatusPending_Returns50Percent()
    {
        // Arrange
        var controller = new ClaimsController();

        // Act
        var result = controller.GetProgressPercentage("Pending");

        // Assert
        Assert.Equal(50, result);
    }
}
