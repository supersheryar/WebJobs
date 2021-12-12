using Xunit;

namespace UkrGuru.WebJobs.Tests
{
    public class UtilityTests
    {
        [Fact()]
        public void IsValidEmailTest()
        {
            // Valid email addresses
            Assert.True(Utility.IsEmailAddress("simple@example.com"), "valid simple@example.com");
            Assert.True(Utility.IsEmailAddress("very.common@example.com"), "valid very.common@example.com");
            Assert.True(Utility.IsEmailAddress("x@example.com"), "valid x@example.com");
            Assert.True(Utility.IsEmailAddress("example-indeed@strange-example.com"), "valid example-indeed@strange-example.com");

            // Invalid email addresses
            Assert.False(Utility.IsEmailAddress(null), "invalid null email");
            Assert.False(Utility.IsEmailAddress(string.Empty), "invalid empty space email");
            Assert.False(Utility.IsEmailAddress(" "), "invalid blank space email");
            Assert.False(Utility.IsEmailAddress("Abc.example.com"), "invalid no @ character");
            Assert.False(Utility.IsEmailAddress("A@b@c@example.com"), "invalid only one @ is allowed outside quotation marks");
            Assert.False(Utility.IsEmailAddress("A\"bc.example.com"), "invalid none of the special characters in this local-part are allowed outside quotation marks");
            //Assert.False(Utility.IsValidEmail("just\"not\"right@example.com"), "quoted strings must be dot separated or the only element making up the local-part");
        }

        //[Fact()]
        //public void IsHtmlBodyTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}
    }
}