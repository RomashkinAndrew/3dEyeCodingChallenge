using Sorter.DataStructures;
using Sorter.Sorters;
using System.Text;
using TestFileGenerator;

namespace UnitTests;

[TestClass]
public sealed class UnitTests
{
    int progress = 0;
    readonly CancellationTokenSource cancellation = new();

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        Directory.CreateDirectory("Test data");
        Directory.SetCurrentDirectory("Test data");
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup()
    {
        Directory.SetCurrentDirectory("..");
        Directory.Delete("Test data", true);
    }

    [TestCleanup]
    public void Cleanup()
    {
        foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
        {
            File.Delete(file);
        }
        foreach (string dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
        {
            Directory.Delete(dir, true);
        }
    }

    [TestMethod]
    public void FileGenerator()
    {
        const int SIZE = 1_000_000;
        const int STRING_LENGTH = 100;
        const int MAX_NUMBER = 999;

        Generator.Generate("data.txt", SIZE, 0.5, STRING_LENGTH, MAX_NUMBER, ref progress, cancellation.Token);
        string[] lineTexts = File.ReadAllLines("data.txt");

        Assert.IsTrue(lineTexts.Sum(s => s.Length) + 2 * lineTexts.Length - 2 >= SIZE);

        List<Line> lines = [];

        foreach (string lineText in lineTexts)
        {
            byte[] lineBytes = Encoding.ASCII.GetBytes(lineText);
            Line line = Line.Parse(lineBytes, 0, lineBytes.Length);
            lines.Add(line);
            Assert.AreEqual(lineText.Length, line.Length);
            Assert.IsTrue(line.IsValid);
            Assert.IsTrue(line.Str.Length <= STRING_LENGTH);
            Assert.IsTrue(line.Number <= MAX_NUMBER);
        }

        Assert.IsTrue(lines.Any(l1 => lines.Any(l2 => l1.Str == l2.Str)));
    }

    [TestMethod]
    public void LineParsing()
    {
        string lineText = "43545363.greshuiosderhghluirtd";
        Line line = Line.Parse(lineText);
        Assert.IsTrue(line.IsValid);
        Assert.IsTrue(line.Str == "greshuiosderhghluirtd");
        Assert.IsTrue(line.Number == 43545363);
    }

    [TestMethod]
    public void LineValidation()
    {
        string lineText1 = "43545363.";
        Line line1 = Line.Parse(lineText1);
        Assert.IsFalse(line1.IsValid);

        string lineText2 = "43545363.greshuiosder\nhghluirtd";
        Line line2 = Line.Parse(lineText2);
        Assert.IsFalse(line2.IsValid);

        string lineText3 = "43545363.greshuiosderhg\rhluirtd";
        Line line3 = Line.Parse(lineText3);
        Assert.IsFalse(line3.IsValid);

        string lineText4 = "43545363.greshuiosderhghluirtd\0";
        Line line4 = Line.Parse(lineText4);
        Assert.IsFalse(line4.IsValid);

        string lineText5 = "43545363.greshuiosde.rhghluirtd";
        Line line5 = Line.Parse(lineText5);
        Assert.IsFalse(line5.IsValid);
    }

    [TestMethod]
    public void FileValidator()
    {
        File.WriteAllText("data.txt",
            "11111111.greshuiosderhghluirt\r\n" +
            "22222222.greshuiosderhghluirt\r\n" +
            "33333333.zzzzzzzzzzzzzzzzzzzz"
            );
        Validator validator = new("data.txt");
        Assert.IsTrue(validator.Validate(cancellation.Token));

        File.WriteAllText("data.txt",
            "22222222.greshuiosderhghluirt\r\n" +
            "11111111.greshuiosderhghluirt\r\n" +
            "33333333.zzzzzzzzzzzzzzzzzzzz"
            );
        Assert.IsFalse(validator.Validate(cancellation.Token));

        File.WriteAllText("data.txt",
            "33333333.zzzzzzzzzzzzzzzzzzzz\r\n" +
            "11111111.greshuiosderhghluirt\r\n" +
            "22222222.greshuiosderhghluirt"
            );
        Assert.IsFalse(validator.Validate(cancellation.Token));

        File.WriteAllText("data.txt",
            "11111111.aaaaaaaaaaaaaaa\naaaaa\r\n" +
            "22222222.aaaaaaaaaaaaaaa\0aaaaa\r\n" +
            "33333333."
            );
        Assert.IsFalse(validator.Validate(cancellation.Token));

    }

    [TestMethod]
    public void DataFileReadWrite()
    {
        Line[] lines1 = [
            Line.Parse("1111.aaaaaaaaaaaaaa"),
            Line.Parse("2222.bbbbbbbbbbbbbb"),
            Line.Parse("3333.cccccccccccccc"),
            Line.Parse("4444.dddddddddddddd"),
            Line.Parse("5555.eeeeeeeeeeeeee"),
            Line.Parse("6666.ffffffffffffff"),
            Line.Parse("7777.gggggggggggggg"),
            Line.Parse("8888.hhhhhhhhhhhhhh"),
            Line.Parse("9999.iiiiiiiiiiiiii"),
        ];

        int size = lines1.Sum(l => l.Length) + lines1.Length * 2 - 2;

        DataFile dataFile = new("data.txt", DataFile.Mode.Write, 100, size);
        foreach (Line line in lines1)
        {
            dataFile.WriteLine(line);
        }
        Assert.AreEqual(size, dataFile.LengthInBytes);
        Assert.AreNotEqual(0, dataFile.BulkWrites);
        dataFile.Dispose();
        dataFile = new("data.txt", DataFile.Mode.Read, 100, size);

        List<Line> lines2 = [];

        while (!dataFile.EndReached)
        {
            lines2.Add(dataFile.ReadLine());
        }

        Assert.AreNotEqual(0, dataFile.BulkReads);
        dataFile.Dispose();
        Assert.AreEqual(lines1.Length, lines2.Count);

        for (int i = 0; i < lines1.Length; i++)
        {
            Assert.AreEqual(lines1[i], lines2[i]);
        }
    }

    [TestMethod]
    public void MergeSort()
    {
        File.WriteAllText("data.txt",
        "22222222.greshuiosderhghluirt\r\n" +
        "11111111.greshuiosderhghluirt\r\n" +
        "33333333.zzzzzzzzzzzzzzzzzzzz"
        );

        MergeSorter sorter = new("data.txt");

        sorter.Sort(cancellation.Token);

        Validator validator = new("data.txt");
        Assert.IsTrue(validator.Validate(cancellation.Token));
    }

    [TestMethod]
    public void HeapSort()
    {
        File.WriteAllText("data.txt",
        "22222222.greshuiosderhghluirt\r\n" +
        "11111111.greshuiosderhghluirt\r\n" +
        "33333333.zzzzzzzzzzzzzzzzzzzz"
        );

        HeapSorter sorter = new("data.txt");

        sorter.Sort(cancellation.Token);

        Validator validator = new("data.txt");
        Assert.IsTrue(validator.Validate(cancellation.Token));
    }
}
