using Microsoft.VisualStudio.TestTools.UnitTesting;

using GF_Operations;

namespace TestProject
{
    [TestClass]
    public class GF256StringOperationsTest
    {
        private static (string str, uint answer)[] _successParsingTestData = new (string str, uint answer)[]
        {
            ("1", 0b1),
            ("x", 0b10),
            ("x^2", 0b100),
            ("x^3", 0b1000),
            ("x^4", 0b10000),
            ("x^5", 0b100000),
            ("x^6", 0b1000000),
            ("x^7", 0b10000000),
            ("x^7 + x^6 + x^5 + x^4 + x^3 + x^2 + x + 1", 0b11111111),
        };

        private static string[] _failParsingTestData = new string[]
        {
            "0",
            "x^8",
            "x^9",
            "x^10",
            "x^111"
        };

        [TestMethod]
        public void PasingTest()
        {
            foreach(var (str, answer) in _successParsingTestData)
            {
                bool success = GF256.TryParse(str, out uint gfElement);

                Assert.AreEqual(true, success, $"succes: expected: {true}, actual: {success}");
                Assert.AreEqual(answer, gfElement, $"gf(256) element: expected: {answer}, actual: {gfElement}");
            }

            foreach (string str in _failParsingTestData)
            {
                bool success = GF256.TryParse(str, out uint gfElement);

                Assert.AreEqual(false, success, $"succes: expected: {false}, actual: {success}");
            }
        }


        private static (uint gfElement, string answer)[] _stringRepresentationTestData = new (uint gfElement, string answer)[]
        {
            (0b1, "1"),
            (0b10, "x"),
            (0b100, "x^2"),
            (0b1000, "x^3"),
            (0b10000, "x^4"),
            (0b100000, "x^5"),
            (0b1000000, "x^6"),
            (0b10000000, "x^7"),
            (0b11111111, "x^7 + x^6 + x^5 + x^4 + x^3 + x^2 + x + 1")
        };

        [TestMethod]
        public void StringRepresentationTest()
        {
            foreach (var (gfElement, answer) in _stringRepresentationTestData)
            {
                string stringRepresentation = GF256.StringRepresentation(gfElement);

                Assert.AreEqual(stringRepresentation, answer, $"gf(256) element: expected: {stringRepresentation}, actual: {stringRepresentation}");
            }
        }
    }
}
