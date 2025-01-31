using SWEET;

namespace TEST;

[TestClass]
public sealed class NonRegOps
{
    [TestMethod]
    public void Rtn()
    {
        byte[] code = [0x00];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.Run == false);
    }

    [TestMethod]
    public void Br()
    {
        byte[] code = [0x01, 0x50];

        Interpreter intp = new(0x0300, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0353);
    }

    [TestMethod]
    public void Bnc()
    {
        byte[] code = [0x02, 0x50];

        Interpreter intp = new(0x0300, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0353);
    }

    [TestMethod]
    public void Bc()
    {
        byte[] code = [0x03, 0x50];

        Interpreter intp = new(0x0300, code);
        intp.Regs[14] = 1;
        
        intp.Start();

        Assert.IsTrue(intp.PC == 0x0353);
    }

    [TestMethod]
    public void Bp()
    {
        byte[] code = [0x12, 0x01, 0x00, 0x04, 0x50];

        Interpreter intp = new(0x0300, code);
        
        intp.Start();
        
        Assert.IsTrue(intp.PC == 0x0356);
    }

    [TestMethod]
    public void Bm()
    {
        byte[] code = [0x12, 0xFF, 0xFF, 0x05, 0x50];

        Interpreter intp = new(0x0300, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0356);
    }


    [TestMethod]
    public void Bz()
    {
        byte[] code = [0x06, 0x50];

        Interpreter intp = new(0x0300, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0353);
    }

    [TestMethod]
    public void Bnz()
    {
        byte[] code = [0x07, 0x50];

        Interpreter intp = new(0x0300, code);
        intp.Regs[13] = 1;
        
        intp.Start();

        Assert.IsTrue(intp.PC == 0x0353);
    }

    [TestMethod]
    public void Bm1()
    {
        byte[] code = [0x12, 0xFF, 0xFF, 0x08, 0x50];

        Interpreter intp = new(0x0300, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0356);
    }

    [TestMethod]
    public void Bnm1()
    {
        byte[] code = [0x12, 0xFF, 0xFF, 0x09, 0x50];

        Interpreter intp = new(0x0300, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0306);
    }

    [TestMethod]
    public void Bk()
    {
        byte[] code = [0x0A, 0x69];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.PC == 0x0003);
    }

    [TestMethod]
    public void Rs()
    {
        //In this section of the original paper the address (320) of MOVE routine is wrong
        //If the PC is at 309 and the offset is 21 (0x15) the correct routine address is 332
        //(PC + 2 + dd)

        byte[] code1 = [0x15, 0x34, 0xA0, 0x14, 0x3B, 0xA0, 0x16, 0x00, 0x30, 0x0C, 0x15];
        byte[] code2 = [0x45, 0x56, 0x24, 0xD5, 0x04, 0xFA, 0x0B];
        byte[] code3 = [0x69, 0x69, 0x69, 0x69, 0x69, 0x69, 0x69, 0x69];

        Interpreter intp = new(300, []);
        Array.Copy(code1, 0, intp.Mem, 300, code1.Length);
        Array.Copy(code2, 0, intp.Mem, 332, code2.Length);
        Array.Copy(code3, 0, intp.Mem, 0xA034, code3.Length);

        intp.Start();

        Assert.IsTrue(intp.PC == 312);

        for (int i = 0; i < 8; i++)
        {
            Console.Write(i);
            Assert.IsTrue(intp.Mem[0xA034 + i] == intp.Mem[0x3000 + i]);
        }
    }

    [TestMethod]
    public void Bs()
    {
        //In this section of the original paper the address (320) of MOVE routine is wrong
        //If the PC is at 309 and the offset is 21 (0x15) the correct routine address is 332
        //(PC + 2 + dd)

        byte[] code1 = [0x15, 0x34, 0xA0, 0x14, 0x3B, 0xA0, 0x16, 0x00, 0x30, 0x0C, 0x15];
        byte[] code2 = [0x45, 0x56, 0x24, 0xD5, 0x04, 0xFA, 0x0B];
        byte[] code3 = [0x69, 0x69, 0x69, 0x69, 0x69, 0x69, 0x69, 0x69];

        Interpreter intp = new(300, []);
        Array.Copy(code1, 0, intp.Mem, 300, code1.Length);
        Array.Copy(code2, 0, intp.Mem, 332, code2.Length);
        Array.Copy(code3, 0, intp.Mem, 0xA034, code3.Length);

        intp.Start();

        Assert.IsTrue(intp.PC == 312);

        for (int i = 0; i < 8; i++)
        {
            Console.Write(i);
            Assert.IsTrue(intp.Mem[0xA034+i] == intp.Mem[0x3000+i]);
        }
    }

}
