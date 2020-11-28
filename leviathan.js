#!/usr/bin/env node

/*
  Leviathan compiler - Full compilation script.
  
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

const path = require('path');
const fs = require('fs');
const { spawn } = require('child_process');

function fileNames(arg) {
  const ext = path.extname(arg);
  const base = path.basename(arg, ext);
  const dir = path.dirname(arg);
  return [path.join(dir, `${base}${ext}`),
          path.join(dir, `${base}.wat`),
          path.join(dir, `${base}.wasm`)];
}

async function watToWasm(watFileName, wasmFileName) {
  try {
    const wabt = await require('wabt')();
    const wasmModule = wabt.parseWat(
      watFileName,
      fs.readFileSync(watFileName, 'utf8'));
    const buffer = wasmModule.toBinary({}).buffer;
    fs.writeFileSync(wasmFileName, new Buffer.from(buffer));
    console.log(`Created WASM (WebAssembly binary) file '${wasmFileName}'.`);

  } catch (error) {
    console.log(error.message);
  }
}

function spawnCompiler(sourceFileName, watFileName, wasmFileName) {
  const subprocess = spawn('mono', ['./leviathan.exe',
                                    sourceFileName,
                                    watFileName]);
  subprocess.stdout.on('data', data => process.stdout.write(data));
  subprocess.stderr.on('data', data => process.stdout.write(data));
  subprocess.on('close', async errorCode => {
    if (errorCode === 0) {
      await watToWasm(watFileName, wasmFileName);
    } else {
      console.log('Oops! Something went wrong.');
    }
  });
}

function main() {
  if (process.argv.length != 3) {
    console.log('Please specify the name of the input file.');
  } else {
    const [sourceFileName, watFileName, wasmFileName] =
      fileNames(process.argv[2]);
    spawnCompiler(sourceFileName, watFileName, wasmFileName);
  }
}

main();
