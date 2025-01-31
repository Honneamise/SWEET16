namespace SWEET;

public class Interpreter
{
    public ushort[] Regs    { get; set; }
    public byte[]   Mem     { get; set; }
    public bool     Run     { get; set; }

    public ushort ACC   { get => Regs[0];   set => Regs[0]  = value; }
    public ushort ROUT  { get => Regs[12];  set => Regs[12] = value; }
    public ushort COMP  { get => Regs[13];  set => Regs[13] = value; }
    public ushort CF    { get => Regs[14];  set => Regs[14] = value; }
    public ushort PC    { get => Regs[15];  set => Regs[15] = value; }

    public Interpreter(ushort entry, byte[] code)
    {
        Run = false;
        Regs = new ushort[16];
        Mem = new byte[0xFFFF];

        Array.Copy(code, 0, Mem, entry, code.Length);

        PC = entry;
    }

    //main execution loop
    public void Start()
    {
        Run = true;

        while (Run)
        {
            Execute();
        }
    }

    //execute a single instruction
    private void Execute()
    {
        byte opcode = Mem[PC];
        PC++;

        //Non reg ops
        if ((opcode & 0xF0) == 0x00)
        {
            switch (opcode & 0x0F)
            {
                case 0x00: Rtn(); break;
                case 0x01: Br(); break;
                case 0x02: Bnc(); break;
                case 0x03: Bc(); break;
                case 0x04: Bp(); break;
                case 0x05: Bm(); break;
                case 0x06: Bz(); break;
                case 0x07: Bnz(); break;
                case 0x08: Bm1(); break;
                case 0x09: Bnm1(); break;
                case 0x0A: Bk(); break;
                case 0x0B: Rs(); break;
                case 0x0C: Bs(); break;

                case 0x0D:
                case 0x0E:
                case 0x0F:

                default: throw new Exception($"Invalid opcode found : {opcode:X}");
            }

        }
        else//Reg ops
        {
            byte reg = (byte)(opcode & 0x0F);

            switch (opcode & 0xF0)
            {
                case 0x10: Set(reg); break;
                case 0x20: Ld(reg); break;
                case 0x30: St(reg); break;
                case 0x40: Ldi(reg); break;
                case 0x50: Sti(reg); break;
                case 0x60: Lddi(reg); break;
                case 0x70: Stdi(reg); break;
                case 0x80: Pop(reg); break;
                case 0x90: Stp(reg); break;
                case 0xA0: Add(reg); break;
                case 0xB0: Sub(reg); break;
                case 0xC0: Popd(reg); break;
                case 0xD0: Cpr(reg); break;
                case 0xE0: Inr(reg); break;
                case 0xF0: Dcr(reg); break;

                default: throw new Exception($"Invalid opcode found : {opcode:X}");
            }
        }      
        
    }

    /*************/
    /* NonRegOps */
    /*************/

    //Return to 6502 mode
    private void Rtn()
    {
        Run = false;
    }

    //Branch always
    private void Br()
    {
        sbyte val = (sbyte)Mem[PC];

        PC++;

        PC = (ushort)(PC + val);
    }
    
    //Branch if no carry
    private void Bnc()
    {
        if (CF == 0) { Br(); }
        else { PC++; }
    }

    //Branch if carry
    private void Bc()
    {
        if (CF == 1) { Br(); }
        else { PC++; }
    }

    //Branch if plus
    private void Bp()
    {
        if ((COMP & 0x8000) == 0) { Br(); }
        else { PC++; }
    }

    //Branch if minus
    private void Bm()
    {
        if ((COMP & 0x8000) == 0x8000) { Br(); }
        else { PC++; }
    }

    //Branch if zero
    private void Bz()
    {
        if (COMP == 0) { Br(); }
        else { PC++; }
    }

    //Branch if not zero
    private void Bnz()
    {
        if (COMP != 0) { Br(); }
        else { PC++; }
    }

    //Branch if -1
    private void Bm1()
    {
        if (COMP == 0xFFFF) { Br(); }
        else { PC++; }
    }

    //Branch if not -1
    private void Bnm1()
    {
        if (COMP != 0xFFFF) { Br(); }
        else { PC++; }
    }

    //Break
    private void Bk()
    {
        PC++;

        Console.WriteLine($"Brk code: 0x{Mem[PC]:X}");
        for (int i = 0; i < Regs.Length/2; i++)
        {
            Console.WriteLine($"R[{i}]: {Regs[i]}\t R[{i + 8}]: {Regs[i + 8]}");
        }
    }

    //Return from subroutine
    private void Rs()
    {
        ROUT -= 2;

        PC = (ushort)( (Mem[ROUT] & 0x00FF) + (Mem[ROUT+1] << 8) );
    }

    //Branch to subroutine
    private void Bs()
    {
        sbyte val = (sbyte)Mem[PC];

        PC++;

        Mem[ROUT] = (byte)((PC) & 0x00FF);
        Mem[ROUT + 1] = (byte)((PC) >> 8);

        ROUT += 2;

        PC = (ushort)(PC + val);

        CF = 0;

        COMP = ACC;

    }

    /**********/
    /* RegOps */
    /**********/

    //Set
    private void Set(byte reg)
    {
        Regs[reg] = (ushort)(Mem[PC] + (Mem[PC+1] << 8));

        COMP = Regs[reg];

        CF = 0;

        PC += 2;
    }

    //Load
    private void Ld(byte reg)
    {
        ACC = Regs[reg];

        COMP = Regs[reg];

        CF = 0;
    }

    //Store
    private void St(byte reg)
    {
        Regs[reg] = ACC;

        COMP = ACC;

        CF = 0;
    }

    //Load indirect
    private void Ldi(byte reg)
    {
        ACC = Mem[Regs[reg]];

        COMP = ACC;

        CF = 0;

        Regs[reg]++;
    }

    //Store indirect
    private void Sti(byte reg)
    {
        Mem[Regs[reg]] = (byte)(ACC & 0x00FF);

        COMP = ACC;

        CF = 0;

        Regs[reg]++;
    }

    //Load double indirect
    private void Lddi(byte reg)
    {
        ACC = Mem[Regs[reg]];

        Regs[reg]++;

        ACC += (ushort)(Mem[Regs[reg]] << 8);

        Regs[reg]++;

        COMP = ACC;

        CF = 0;
    }

    //Store double indirect
    private void Stdi(byte reg)
    {
        Mem[Regs[reg]] = (byte)(ACC & 0x00FF);

        Regs[reg]++;

        Mem[Regs[reg]] = (byte)(ACC >> 8);

        Regs[reg]++;

        COMP = ACC;

        CF = 0;
    }

    //Pop indirect
    private void Pop(byte reg)
    {
        Regs[reg]--;

        ACC = Mem[Regs[reg]];

        COMP = ACC;

        CF = 0;
    }

    //Store pop indirect
    private void Stp(byte reg)
    {
        Regs[reg]--;

        Mem[Regs[reg]] = (byte)(ACC & 0x00FF);

        Regs[reg]--;

        Mem[Regs[reg]] = (byte)(ACC >> 8);

        COMP = ACC;

        CF = 0;
    }

    //Add
    private void Add(byte reg)
    {
        CF = (ACC + Regs[reg] > 0xFFFF) ? (ushort)0x01 : (ushort)0x00;

        ACC += Regs[reg];

        COMP = ACC;
    }

    //Subtract
    private void Sub(byte reg)
    {
        //In this section of the original paper the description of the Sub operation is wrong.
        //The CF flag should be set only if ACC is "strictly" greater then RN.
        CF = ( ACC > Regs[reg]) ? (ushort)0x01 : (ushort)0x00;

        ACC -= Regs[reg];
        
        COMP = ACC;
    }

    //Pop double byte indirect
    private void Popd(byte reg)
    {
        Regs[reg]--;

        ACC = (ushort)(Mem[Regs[reg]] << 8);

        Regs[reg]--;

        ACC += Mem[Regs[reg]];

        COMP = ACC;

        CF = 0;
    }

    //Compare
    private void Cpr(byte reg)
    {
        COMP = (ushort)(ACC - Regs[reg]);

        CF = (ACC >= Regs[reg]) ? (ushort)0x01 : (ushort)0x00;
    }

    //Increment
    private void Inr(byte reg)
    {
        Regs[reg]++;

        COMP = Regs[reg];

        CF = 0;
    }

    //Decrement
    private void Dcr(byte reg)
    {
        Regs[reg]--;

        COMP = Regs[reg];

        CF = 0;
    }

}

