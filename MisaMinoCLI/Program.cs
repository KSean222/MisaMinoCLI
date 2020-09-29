using System;
using System.IO;
using System.Linq;
using System.Timers;
using MisaMinoNET;
using Newtonsoft.Json;

namespace MisaMinoCLI
{
    class MisaMinoArgs
    {
        public char[] Queue { get; set; }
        public char Current { get; set; }
        public char? Hold { get; set; }
        public int Height { get; set; }
        public bool[][] Field { get; set; }
        public int Combo { get; set; }
        public bool B2b { get; set; }
        public int Garbage { get; set; }
        public int MaxThinkTime { get; set; }
    }
    class MisaMinoResult
    {
        public Solution Ok { get; set; }
    }
    class Program
    {
        static int ToPiece(char piece) {
            switch (piece) {
                case 'S': return 0;
                case 'Z': return 1;
                case 'J': return 2;
                case 'L': return 3;
                case 'T': return 4;
                case 'O': return 5;
                case 'I': return 6;
                default: throw new InvalidOperationException();
            }
        }
        static void Main() {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(4096)));
            MisaMino.Finished += success => {
                if (success) {
                    MisaMinoResult result = new MisaMinoResult {
                        Ok = MisaMino.LastSolution
                    };
                    Console.WriteLine(JsonConvert.SerializeObject(result));
                } else {
                    Console.WriteLine("{\"Err\":null}");
                }
            };
            JsonSerializerSettings settings = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            while (true) {
                string line = Console.ReadLine();
                if (line == null) {
                    break;
                }
                line = line.Trim();
                if (line == "\"abort\"") {
                    MisaMino.Abort();
                } else {
                    MisaMinoArgs args = JsonConvert.DeserializeObject<MisaMinoArgs>(line, settings);
                    int[,] field = new int[10, 40];
                    for (int x = 0; x < 10; x++) {
                        for (int y = 0; y < 40; y++) {
                            field[x, y] = args.Field[y][x] ? 0 : 255;
                        }
                    }
                    MisaMino.FindMove(
                        args.Queue.Select(ToPiece).ToArray(),
                        ToPiece(args.Current),
                        args.Hold.HasValue ? (int?)ToPiece(args.Hold.Value) : null,
                        args.Height,
                        field,
                        args.Combo,
                        args.B2b ? 1 : 0,
                        args.Combo
                    );
                }                
            }
        }
    }
}
