namespace SWEET;

public class Assembler
{
    //name + number of params + base opcode
    private static readonly Dictionary<string, (int args, byte opcode)> TABLE = new()
    {
        //macro
        { "FILL",   (1, 0x00) },

        //Non reg ops
        { "RTN",    (0, 0x00) },//no params
        { "BR",     (1, 0x01) },
        { "BNC",    (1, 0x02) },
        { "BC",     (1, 0x03) },
        { "BP",     (1, 0x04) },
        { "BM",     (1, 0x05) },
        { "BZ",     (1, 0x06) },
        { "BNZ",    (1, 0x07) },
        { "BM1",    (1, 0x08) },
        { "BNM1",   (1, 0x09) },
        { "BK",     (1, 0x0A) },
        { "RS",     (0, 0x0B) },//no params
        { "BS",     (1, 0x0C) },

        //Reg ops
        { "SET",    (2, 0x10) },
        { "LD",     (1, 0x20) },//or 0x40 if indirect
        { "ST",     (1, 0x30) },//or 0x50 if indirect
        { "LDD",    (1, 0x60) },//ind
        { "STD",    (1, 0x70) },//ind
        { "POP",    (1, 0x80) },//ind
        { "STP",    (1, 0x90) },//ind
        { "ADD",    (1, 0xA0) },
        { "SUB",    (1, 0xB0) },
        { "POPD",   (1, 0xC0) },//ind
        { "CPR",    (1, 0xD0) },
        { "INR",    (1, 0xE0) },
        { "DCR",    (1, 0xF0) },
    };

    //convert string to ushort
    //if limit == true, limit the size to byte
    private static bool StrToNumber(string str, bool limit, out ushort num)
    {
        try
        {
            int bas = str.StartsWith("0x") ? 16 : 10;

            ushort value = Convert.ToUInt16(str, bas);

            if (limit && value > byte.MaxValue)
            {
                num = 0;
                return false;
            }

            num = value;
            return true;
        }
        catch
        {
            num = 0;
            return false;
        }
    }

    //Consume tokens to encode the instruction and update labels references
    private static byte[] Encode(Queue<string> tokens, List<(string,ushort)> references, ushort address)
    {
        string ins = tokens.Dequeue();

        switch (ins)
        {
            //macro
            case "FILL":
                {
                    if (StrToNumber(tokens.Dequeue(), false, out ushort size))
                    {
                        return new byte[size];
                    }
                    break;
                }

            //ins
            case "RTN": 
            case "RS":
                {
                    return new byte[] { TABLE[ins].opcode };
                }

            //ins + address
            case "BR":
            case "BNC":
            case "BC":
            case "BP":
            case "BM":
            case "BZ":
            case "BNZ":
            case "BM1":
            case "BNM1":
            case "BK":
            case "BS":
                {
                    string str = tokens.Dequeue();

                    if (StrToNumber(str, true, out ushort addr))
                    {
                        if ((sbyte)addr < -128 || (sbyte)addr > 127) { break; }

                        return new byte[] { TABLE[ins].opcode, (byte)addr };
                    }

                    references.Add((str, (ushort)(address+1) ));

                    return new byte[] { TABLE[ins].opcode, 0x00 };    
                }
            
            //ins + reg
            case "ADD":
            case "SUB":
            case "CPR":
            case "INR":
            case "DCR":
                {
                    string str = tokens.Dequeue();

                    if (str.StartsWith("R") && StrToNumber(str[1..], true, out ushort reg))
                    {
                        if (reg > 15) { break; }

                        return new byte[] { (byte)(TABLE[ins].opcode + reg) };
                    }
                    break;
                }

            //ins + reg indirect
            case "LDD":
            case "STD":
            case "POP":
            case "STP":
            case "POPD":
                {
                    string str = tokens.Dequeue();

                    if (str.StartsWith("@R") && StrToNumber(str[2..], true, out ushort reg))
                    {
                        if (reg > 15) { break; }

                        return new byte[] { (byte)(TABLE[ins].opcode + reg) };
                    }
                    break;
                }

            //ins + (reg OR reg indirect)
            case "LD":
            case "ST":
                {
                    string str = tokens.Dequeue();

                    if (str.StartsWith("R") && StrToNumber(str[1..], true, out ushort reg))
                    {
                        if (reg > 15) { break; }

                        return new byte[] { (byte)(TABLE[ins].opcode + reg) };
                    }

                    if (str.StartsWith("@R") && StrToNumber(str[2..], true, out ushort regind))
                    {
                        if (regind > 15) { break; }

                        return new byte[] { (byte)((TABLE[ins].opcode + 0x20) + regind) };
                    }
                    break;
                }

            //ins + reg + value
            case "SET":
                {
                    string str = tokens.Dequeue();

                    if (str.StartsWith("R") && StrToNumber(str[1..], true, out ushort reg))
                    {
                        if (reg > 15) { break; }

                        if (StrToNumber(tokens.Dequeue(), false, out ushort val))
                        {
                            return new byte[] { (byte)(TABLE[ins].opcode + (byte)reg), (byte)(val & 0x00FF), (byte)(val >> 8) };
                        }
                    }
                    break;
                }

            //err
            default: break;
        }

        return Array.Empty<byte>();
    }

    //code generator
    //for each line create a queue of tokens
    //for queue check in order for : label, org, instruction, comment
    //last loop update labels reference with value
    public static byte[] Generate(string text)
    {
        string[] lines = text.Split(Environment.NewLine);

        Dictionary<string, ushort> labels = new();
        List<(string, ushort)> references = new();

        byte[] code = Array.Empty<byte>();

        ushort org = 0x0000;
        ushort address = 0x0000;

        //for each line create a stack of tokens
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) { continue; }

            string[] toks = lines[i].Split(new char[] { ' ', ',', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (toks.Length == 0) { continue; }

            Queue<string> tokens = new(toks);

            //label ?
            if (char.IsLetter(tokens.Peek()[0]) && tokens.Peek().EndsWith(":"))
            {
                string lab = tokens.Dequeue()[..^1];

                //valid label format ?
                if (!lab.All(c => char.IsLetterOrDigit(c) && (!char.IsLetter(c) || char.IsUpper(c))) || TABLE.ContainsKey(lab))
                {
                    throw new Exception($"Invalid label at line : {i}");
                }

                if (labels.ContainsKey(lab))
                {
                    throw new Exception($"Duplicate label at line : {i}");
                }

                labels[lab] = address;

                if (tokens.Count == 0) { continue; }
            }

            //org
            if (tokens.Peek().Equals("ORG"))
            {
                if (address != 0) { throw new Exception($"Entry point already set : {i}"); }

                tokens.Dequeue();

                if (StrToNumber(tokens.Dequeue(), false, out ushort adr))
                {
                    address = adr;
                    org = adr;
                }

                if (tokens.Count == 0 || tokens.Peek().StartsWith("*")) { continue; }

                throw new Exception($"Invalid line : {i}"); 
            }

            //instruction ?
            if (TABLE.ContainsKey(tokens.Peek()) && tokens.Count >= 1 + TABLE[tokens.Peek()].args)
            {
                byte[] result = Encode(tokens, references, address);

                if (result.Length == 0) { throw new Exception($"Malformed instruction at line : {i}"); }

                address += (ushort)result.Length;

                code = code.Concat(result).ToArray();

                if (tokens.Count == 0) { continue; }
            }

            //comment ?
            if (tokens.Peek().StartsWith("*")) { continue; }

            //error
            throw new Exception($"Invalid line : {i}");
        }

        //labels references
        foreach ((string lab, ushort add) in references)
        {
            if (!labels.ContainsKey(lab))
            {
                throw new Exception($"Missing label reference : {lab}");
            }

            int gap = (labels[lab] - add) - 1;

            if (gap < -128 || gap > 127) 
            { 
                throw new Exception($"Label reference out of range : {lab}"); 
            }

            code[add-org] = (byte)((sbyte)gap);
        }

        return code;
    }

}
