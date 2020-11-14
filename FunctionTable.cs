/*
  Leviathan compiler - Semantic Analysis
    Symbol table class.

  Camila Rovirosa A010241927
  Eduardo Badillo A01020716
  Isabel Maqueda  A01652906

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
using System.Text;
using System.Collections.Generic;

// Function table en lugar de type 
// necesitamos una Tabla de Variables Globales 

namespace Leviathan {

    public class FunctionRow {
        public bool primitive { get; private set; }
        public int arity { get; private set; }
        public HashSet<string> reference { get; set; }

        public FunctionRow(bool primitive, int arity, HashSet<string> localSymbolTable){
            this.primitive = primitive;
            this.arity = arity; 
            this.reference = localSymbolTable;
        }

        public override string ToString(){
            var localVarTable = "";
            // TODO: Append local var table
            return $"{this.primitive}, {this.arity} {localVarTable}";

        }
    }

    public class FunctionTable: IEnumerable<KeyValuePair<string, FunctionRow>> { 

        IDictionary<string, FunctionRow> data = new SortedDictionary<string, FunctionRow>();

        //-----------------------------------------------------------
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Function Table\n");
            sb.Append("====================\n");
            foreach (var entry in data) {
                sb.Append($"{entry.Key}: {entry.Value}\n");
            }
            sb.Append("====================\n");
            return sb.ToString();
        }

        //-----------------------------------------------------------
        public FunctionRow this[string key] {
            get {
                return data[key];
            }
            set {
                data[key] = value;
            }
        }
        //-----------------------------------------------------------
        public bool Contains(string key) {
            return data.ContainsKey(key);
        }
        //-----------------------------------------------------------
        public IEnumerator<KeyValuePair<string, FunctionRow>> GetEnumerator() {
            return data.GetEnumerator();
        }
        //-----------------------------------------------------------
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }

}
