/*
    Levithan Compiler - Semantic Analysis

    Camila Rovirosa A01024192
    Eduardo Badillo A01020716
    Isabel Maqueda  A01652906

  Leviathan compiler - Program driver.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.IO;
using System.Text;

namespace Leviathan {
    public class Driver {
        const string VERSION = "0.4";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic Analysis"
        };

         void PrintAppHeader() {
             Console.WriteLine("Leviathan compiler, version " + VERSION);
              Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This program has absolutely no warranty.");
         }

         void PrintReleaseIncludes() {
            Console.WriteLine("Included in this release:");
            foreach (var phase in ReleaseIncludes) {
                Console.WriteLine("   * " + phase);
            }
        }
        void Run (string[] args){
            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();
            
            // Input file
            if (args.Length != 1) {
                Console.Error.WriteLine(
                    "Please specify the name of the input file.");
                Environment.Exit(1);
            }

            try {
                var inputPath = args[0];
                var input = File.ReadAllText(inputPath);
                /*Console.WriteLine(String.Format(
                    "===== Tokens from: \"{0}\" =====", inputPath)
                );
                var count = 1;
                foreach (var tok in new Scanner(input).Start()) {
                    Console.WriteLine(String.Format("[{0}] {1}",
                                                    count++, tok)
                    );*/

                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program = parser.Program();
                Console.WriteLine("Syntax OK.");
                //Console.Write(program.ToStringTree());

                var semantic = new SemanticVisitor();
                semantic.Visit((dynamic) program);

                Console.WriteLine("Semantics OK.");
                Console.WriteLine();
                Console.WriteLine("Symbol Table");
                Console.WriteLine("============");
                foreach (var entry in semantic.Table) {
                    Console.WriteLine(entry);
                }

            } catch (Exception e) {

                if (e is FileNotFoundException || e is SyntaxError|| e is SemanticError) {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }

                throw;
            }
        }

        public static void Main(string[] args) {
            new Driver().Run(args);
        }
    }
}