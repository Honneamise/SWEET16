using SWEET;

namespace TEST;


[TestClass]
public sealed class Generator
{
    [TestMethod]
    public void Test1()
    {
        string text = @"
        ORG 0x030A
    MLOOP: LD @R1
        ST @R2
        DCR R3
        BNZ MLOOP
        RTN
        ";

        byte[] res = [
            0x41,
            0x52,
            0xF3,
            0x07, 0xFB,
            0x00];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test2()
    {
        string text = "SET R5 0xA034";

        byte[] res = [0x15, 0x34, 0xA0];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test3()
    {
        string text = @"
        SET R5 0xA034
        LD R5";

        byte[] res = [0x15, 0x34, 0xA0, 0x25];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test4()
    {
        string text = @"
        LD R5
        ST R6";

        byte[] res = [0x25, 0x36];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test5()
    {
        string text = @"
        SET R5 0xA034
        LD @R5";

        byte[] res = [0x15, 0x34, 0xA0, 0x45];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test6()
    {
        string text = @"
        SET R5 0xA034
        SET R6 0x9022
        LD @R5
        ST @R6";

        byte[] res = [
            0x15, 0x34, 0xA0, 
            0x16, 0x22, 0x90,
            0x45,
            0x56];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test7()
    {
        string text = @"
        SET R5 0xA034
        LDD @R5";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x65];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test8()
    {
        string text = @"
        SET R5 0xA034
        SET R6 0x9022
        LDD @R5
        STD @R6";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x16, 0x22, 0x90,
            0x65,
            0x76];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test9()
    {
        string text = @"
        SET R5 0xA034
        SET R0 4
        ST @R5
        SET R0 5
        ST @R5
        SET R0 6
        ST @R5
        POP @R5
        POP @R5
        POP @R5
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x10, 0x04, 0x00,
            0x55,
            0x10, 0x05, 0x00,
            0x55,
            0x10, 0x06, 0x00,
            0x55,
            0x85,
            0x85,
            0x85];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test10()
    {
        string text = @"
        SET R4 0xA034
        SET R5 0x9022
        POP @R4
        STP @R5
        POP @R4
        STP @R5
        ";

        byte[] res = [
            0x14, 0x34, 0xA0,
            0x15, 0x22, 0x90,
            0x84,
            0x95,
            0x84,
            0x95];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test11()
    {
        string text = @"
        SET R0 0x7634
        SET R1 0x4227
        ADD R1
        ADD R0
        ";

        byte[] res = [
            0x10, 0x34, 0x76,
            0x11, 0x27, 0x42,
            0xA1,
            0xA0];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test12()
    {
        string text = @"
        SET R0 0x7634
        SET R1 0x4227
        SUB R1
        SUB R0
        ";

        byte[] res = [
            0x10, 0x34, 0x76,
            0x11, 0x27, 0x42,
            0xB1,
            0xB0];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }


    [TestMethod]
    public void Test13()
    {
        string text = @"
        SET R5 0xA034
        SET R0 0xAA12
        STD @R5
        SET R0 0xBB34
        STD @R5
        SET R0 0xCC56
        STD @R5
        POPD @R5
        POPD @R5
        POPD @R5
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x10, 0x12, 0xAA,
            0x75,
            0x10, 0x34, 0xBB,
            0x75,
            0x10, 0x56, 0xCC,
            0x75,
            0xC5,
            0xC5,
            0xC5];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test14()
    {
        string text = @"
        SET R5 0xA034
        SET R6 0xA0BF
    LOOP: SET R0 0
        STD @R5
        LD R5
        CPR R6
        BNC LOOP
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x16, 0xBF, 0xA0,
            0x10, 0x00, 0x00,
            0x75,
            0x25,
            0xD6,
            0x02, 0xF8];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test15()
    {
        string text = @"
        SET R5 0xA034
        SET R0 0
        ST @R5
        INR R5
        ST @R5
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x10, 0x00, 0x00,
            0x55,
            0xE5,
            0x55];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test16()
    {
        string text = @"
        SET R5 0xA034
        SET R4 9
        SET R0 0
    LOOP: ST @R5
        DCR R4
        BNZ LOOP
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x14, 0x09, 0x00,
            0x10, 0x00, 0x00,
            0x55,
            0xF4,
            0x07, 0xFC];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test17()
    {
        string text = @"
        SET R5 0xA034
        SET R4 0xA03F
    LOOP: SET R0 0
        ST @R5
        LD R4
        CPR R5
        BP LOOP
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x14, 0x3F, 0xA0,
            0x10, 0x00, 0x00,
            0x55,
            0x24,
            0xD5,
            0x04, 0xF8];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    [TestMethod]
    public void Test18()
    {
        string text = @"
    ORG 300
        SET R5 0xA034
        SET R4 0xA03B
        SET R6 0x3000
        BS MOVE

        FILL 21

    MOVE: LD @R5
        ST @R6
        LD R4
        CPR R5
        BP MOVE
        RS
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x14, 0x3B, 0xA0,
            0x16, 0x00, 0x30,
            0x0C, 0x15,

            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00,

            0x45,
            0x56,
            0x24,
            0xD5,
            0x04, 0xFA,
            0x0B];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }

    /**************************/
    [TestMethod]
    public void Test_()
    {
        string text = @"
        SET R5 0xA034 *Init pointer.
        SET R4 0xA03F *Init limit.
        LOOP: SET R0 0
        ST @R5
        LD R4
        CPR R5
        BP LOOP
        ";

        byte[] res = [
            0x15, 0x34, 0xA0,
            0x14, 0x3F, 0xA0,
            0x10, 0x00, 0x00,
            0x55, 0x24,
            0xD5,
            0x04, 0xF8];

        byte[] code = Assembler.Generate(text);

        Assert.IsTrue(code.SequenceEqual(res));
    }
}