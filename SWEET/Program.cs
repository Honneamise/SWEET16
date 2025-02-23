﻿namespace SWEET;

public class Program
{
    private static void Usage()
    {
        Console.WriteLine("USAGE: SWEET.EXE [params]");
        Console.WriteLine("Params list:");
        Console.WriteLine("-b srcfile dstfile   [call the assembler]");
        Console.WriteLine("-e entry binfile     [execute the interpreter]");
    }

    public static void Main(string[] args)
    {
        //call assembler
        if (args.Length == 3 && args[0] == "-b")
        {
            string text = File.ReadAllText(args[1]);

            byte[] code = Assembler.Generate(text);

            File.WriteAllBytes(args[2], code);

            return;
        }

        //call interpreter
        if (args.Length == 3 && args[0] == "-e")
        {
            int bas = args[1].StartsWith("0x") ? 16 : 10;

            ushort entry = Convert.ToUInt16(args[1], bas);

            byte[] code = File.ReadAllBytes(args[2]);

            Interpreter intp = new(entry, code);

            intp.Start();

            return;
        }

        Usage();
    }
}