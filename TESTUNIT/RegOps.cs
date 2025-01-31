using SWEET;

namespace TEST;


[TestClass]
public sealed class RegOps
{
    [TestMethod]
    public void Set()
    {
        byte[] code = [0x15, 0x34, 0xA0];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.Regs[5] == 0xA034);
        Assert.IsTrue(intp.PC == 4);
    }

    [TestMethod]
    public void Ld()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x25];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.ACC == 0xA034);
    }

    [TestMethod]
    public void St()
    {
        byte[] code = [0x25, 0x36];

        Interpreter intp = new(0x0000, code);
        intp.Regs[5] = 0x3369;
        
        intp.Start();

        Assert.IsTrue(intp.Regs[6] == 0x3369);
    }

    [TestMethod]
    public void Ldi()
    {        
        byte[] code = [0x15, 0x34, 0xA0, 0x45];

        Interpreter intp = new(0x0000, code);
        intp.Mem[0xA034] = 0x69;

        intp.Start();

        Assert.IsTrue(intp.ACC == 0x0069);
        Assert.IsTrue(intp.Regs[5] == 0xA035);
    }

    [TestMethod]
    public void Sti()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x16, 0x22, 0x90, 0x45, 0x56];

        Interpreter intp = new(0x0000, code);
        intp.Mem[0xA034] = 0x69;

        intp.Start();

        Assert.IsTrue(intp.Mem[0x9022] == 0x69);
        Assert.IsTrue(intp.Regs[5] == 0xA035);
        Assert.IsTrue(intp.Regs[6] == 0x9023);
    }

    [TestMethod]
    public void Lddi()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x65];

        Interpreter intp = new(0x0000, code);
        intp.Mem[0xA034] = 0x33;
        intp.Mem[0xA035] = 0x69;

        intp.Start();

        Assert.IsTrue(intp.ACC == 0x6933);
        Assert.IsTrue(intp.Regs[5] == 0xA036);
    }

    [TestMethod]
    public void Stdi()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x16, 0x22, 0x90, 0x65, 0x76];
        
        Interpreter intp = new(0x0000, code);
        intp.Mem[0xA034] = 0x33;
        intp.Mem[0xA035] = 0x69;

        intp.Start();

        Assert.IsTrue(intp.Mem[0x9022] == 0x33);
        Assert.IsTrue(intp.Mem[0x9023] == 0x69);
        Assert.IsTrue(intp.Regs[5] == 0xA036);
        Assert.IsTrue(intp.Regs[6] == 0x9024);
    }

    [TestMethod]
    public void Pop()
    {
        //In this section of the original paper the Store Indirect opcodes are wrong.
        //0x35 opcodes should be instead 0x55
        byte[] code = [0x15, 0x34, 0xA0, 0x10, 0x04, 0x00, 0x55, 0x10, 0x05, 
                       0x00, 0x55, 0x10, 0x06, 0x00, 0x55, 0x85, 0x85, 0x85];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.ACC == 0x0004);
        Assert.IsTrue(intp.Regs[5] == 0xA034);
    }

    [TestMethod]
    public void Stp()
    {
        //In this section of the original paper the assembly description is wrong.
        //The byte at 0xA033 will be moved at 0x9021
        //The byte at 0xA032 will be moved at 0x901F
        byte[] code = [0x14, 0x34, 0xA0, 0x15, 0x22, 0x90, 0x84, 0x95, 0x84, 0x95];

        Interpreter intp = new(0x0000, code);
        intp.Mem[0xA032] = 0x33;
        intp.Mem[0xA033] = 0x69;

        intp.Start();

        Assert.IsTrue(intp.Mem[0x9021] == 0x69);
        Assert.IsTrue(intp.Mem[0x9020] == 0x00);
        Assert.IsTrue(intp.Mem[0x901F] == 0x33);
        Assert.IsTrue(intp.Mem[0x901E] == 0x00);
    }

    [TestMethod]
    public void Add()
    {
        byte[] code = [0x10, 0x34, 0x76, 0x11, 0x27, 0x42, 0xA1, 0xA0];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.ACC == 0x70B6);
        Assert.IsTrue(intp.CF == 1);
        Assert.IsTrue(intp.PC == 9);
    }

    [TestMethod]
    public void Sub()
    {
        //In this section of the original paper the Sub opcodes are wrong.
        //0xA1 opcode should be instead 0xB1
        //0xA0 opcode should be instead 0xB0
        byte[] code = [0x10, 0x34, 0x76, 0x11, 0x27, 0x42, 0xB1, 0xB0];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.ACC == 0x0000);
        Assert.IsTrue(intp.CF == 0);
        Assert.IsTrue(intp.PC == 9);
    }

    [TestMethod]
    public void Popd()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x10, 0x12, 0xAA, 0x75, 0x10, 0x34,
                       0xBB, 0x75, 0x10, 0x56, 0xCC, 0x75, 0xC5, 0xC5, 0xC5];

        Interpreter intp = new(0x0000, code);

        intp.Start();

        Assert.IsTrue(intp.ACC == 0xAA12);
        Assert.IsTrue(intp.PC == 19);
    }

    [TestMethod]
    public void Cpr()
    {
        byte[] code = [0xD6];

        Interpreter intp = new(0x0000, code);
        intp.Regs[0] = 0x69;
        intp.Regs[6] = 0x69;
        
        intp.Start();

        Assert.IsTrue(intp.COMP == 0x0000);
        Assert.IsTrue(intp.CF == 1);
        Assert.IsTrue(intp.PC == 2);
    }

    [TestMethod]
    public void Inr()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x10, 0x00, 0x00, 0x55, 0xE5, 0x55];
        byte[] data = [0x69, 0x69, 0x69, 0x69, 0x69, 0x69, 0x69, 0x69];

        Interpreter intp = new(0x0000, []);
        Array.Copy(code, 0, intp.Mem, 0, code.Length);
        Array.Copy(data, 0, intp.Mem, 0xA034, data.Length);

        intp.Start();

        Assert.IsTrue(intp.Mem[0xA034] == 0x00);
        Assert.IsTrue(intp.Mem[0xA035] == 0x69);
        Assert.IsTrue(intp.Mem[0xA036] == 0x00);
        Assert.IsTrue(intp.Mem[0xA037] == 0x69);

        Assert.IsTrue(intp.PC == 10);
    }

    [TestMethod]
    public void Dcr()
    {
        byte[] code = [0x15, 0x34, 0xA0, 0x14, 0x09, 0x00, 0x10, 
                       0x00, 0x00, 0x55, 0xF4, 0x07, 0xFC];

        byte[] data = [0x69, 0x69, 0x69, 0x69, 0x69,
                       0x69, 0x69, 0x69, 0x69, 0x69];

        Interpreter intp = new(0x0000, []);
        Array.Copy(code, 0, intp.Mem, 0, code.Length);
        Array.Copy(data, 0, intp.Mem, 0xA034, data.Length);

        intp.Start();

        for (int i = 0; i < 9; i++)
        {
            Assert.IsTrue(intp.Mem[0xA034 + i] == 0x00);
        }

        Assert.IsTrue(intp.PC == 14);
    }
}
