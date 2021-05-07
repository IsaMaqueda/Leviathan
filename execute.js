#!/usr/bin/env node

/*
  Leviathan WASM execution script.
  
    Camila Rovirosa A01024192
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

const fs = require('fs');
const levilib = require('./leviathanlib');

async function main() {
  try {
    if (process.argv.length != 3) {
      console.log('Please specify the name of the WASM file to execute.');
      process.exit(1);
    }
    const fileName = process.argv[2];
    const buffer = fs.readFileSync(fileName);
    const module = await WebAssembly.compile(buffer);
    const instance = await WebAssembly.instantiate(module, levilib);
    instance.exports.main();
  } catch (error) {
    console.log(error.message);
  }
}

main();
