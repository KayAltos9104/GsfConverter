namespace GsfConverter.Tests
{
    [TestClass]
    public class GsfConverterTests
    {
        [TestMethod]
        public void ReadFile_PointsList()
        {
            // arrange
            var path = "Accreation_ASCII_1.rawst";
            var expected = new List<(double X, double Y, double Z)>();

            // act
            var actualList = Converter.ReadStructure(path);

            // assert
            Assert.AreEqual(expected.GetType(), actualList.GetType());
        }

        [TestMethod]
        public void ADConvert_DiscretePointsList()
        {
            // arrange
            var path = "Accreation_ASCII_1.rawst";
            var expected = new List<(int X, int Y, int Z)>();

            // act
            var actualList = Converter.ADConvert(Converter.ReadStructure(path), 10000);              

            // assert
            Assert.AreEqual(expected.GetType(), actualList.GetType());
        }
        [TestMethod]
        public void SaveStructure_GsfFile()
        {
            // arrange
            var path = "Accreation_ASCII_1.rawst";
            var pathGsf = "NoseTest.gsf";
            // act
            var gsfList = Converter.ADConvert(Converter.ReadStructure(path), 10000);
            Converter.SaveToGsf(pathGsf, gsfList);

            // assert
            Assert.IsTrue(File.Exists(pathGsf));
        }
    }
}