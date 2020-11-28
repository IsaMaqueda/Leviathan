/*
  Leviathan API.
  Copyright (C) 2020 Ariel Ortiz, ITESM CEM
    Camila Rovirosa A01024192
    Eduardo Badillo A01020716
    Isabel Maqueda  A01652906

  Make sure to install the readline-sync module in your system.
  At the terminal, type:

      npm install readline-sync

  To use the module from Node:

      const lib = require('./leviathanlib');
      lib.leviathan.printi(42);

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
const os = require('os');
const readlineSync = require('readline-sync');

const invalidHandleError = 'Invalid array handle.';
const invalidBoundsError = 'Array index out of bounds: ';
const endOfLine = os.EOL;
const handles = [];
const validIntRegEx = /^\s*-?\d+\s*$/;

/**
 * Local function that checks if i is within bounds of array a,
 * otherwise throws an exception with a given errorMessage.
 */
function checkBounds(a, i, errorMessage) {
  if (!(0 <= i && i < a.length)) {
    throw new Error(errorMessage);
  }
}

module.exports = {

  leviathan: {

    /**
     * Prints i to stdout as a decimal integer. Does not print a
     * new line at the end. Returns 0.
     */
    printi(i) {
      fs.writeSync(process.stdout.fd, String(i));
      return 0;
    },

    /**
     * Prints a character to stdout, where c is its Unicode code
     * point. Does not print a new line at the end. Returns 0.
     */
    printc(c) {
      fs.writeSync(process.stdout.fd, String.fromCodePoint(c));
      return 0;
    },

    /**
     * Prints s to stdout as a string. s must be a handle to an
     * array list containing zero or more Unicode code points.
     * Does not print a new line at the end. Returns 0.
     */
    prints(s) {
      checkBounds(handles, s, invalidHandleError);
      const builder = [];
      for (const c of handles[s]) {
        builder.push(String.fromCodePoint(c));
      }
      fs.writeSync(process.stdout.fd, builder.join(''));
      return 0;
    },

    /**
     * Prints a newline character to stdout. Returns 0.
     */
    println() {
      fs.writeSync(process.stdout.fd, endOfLine);
      return 0;
    },

    /**
     * Reads from stdin a signed decimal integer and return its
     * value. Does not return until a valid integer has been read.
     */
    readi() {
      let input;
      do {
        input = readlineSync.question();
      } while (!validIntRegEx.test(input));

      return Number(input);
    },

    /**
     * Reads from stdin a string (until the end of line) and returns
     * a handle to a newly created array list containing the Unicode
     * code points of all the characters read, excluding the end of
     * line.
     */
    reads() {
      const input = readlineSync.question();
      const arrayList = [];
      for (let i = 0; i < input.length; i++) {
        arrayList.push(input.codePointAt(i));
      }
      handles.push(arrayList);
      return handles.length - 1;
    },

    /**
     * Creates a new array list object with n elements and returns
     * its handle. All the elements of the array list are set to
     * zero. Throws an exception if n is less than zero.
     */
    new(n) {
      if (n < 0) {
        throw new Error("Can't create a negative size array.");
      }
      const arrayList = new Array(n);
      for (let i = 0; i < n; i++) {
        arrayList[i] = 0;
      }
      handles.push(arrayList);
      return handles.length - 1;
    },

    /**
     * Returns the size (number of elements) of the array list
     * referenced by handle h. Throws an exception if h is not
     * a valid handle.
     */
    size(h) {
      checkBounds(handles, h, invalidHandleError);
      return handles[h].length;
    },

    /**
     * Adds x at the end of the array list referenced by handle h.
     * Returns 0. Throws an exception if h is not a valid handle.
     */
    add(h, x) {
      checkBounds(handles, h, invalidHandleError);
      handles[h].push(x);
      return 0;
    },

    /**
     * Returns the value at index i from the array list referenced by
     * handle h. Throws an exception if i is out of bounds or if h is
     * not a valid handle.
     */
    get(h, i) {
      checkBounds(handles, h, invalidHandleError);
      checkBounds(handles[h], i, invalidBoundsError + i);
      return handles[h][i];
    },

    /**
     * Sets to x the element at index i of the array list referenced
     * by handle h. Returns 0. Throws an exception if i is out of
     * bounds or if h is not a valid handle.
     */
    set(h, i, x) {
      checkBounds(handles, h, invalidHandleError);
      checkBounds(handles[h], i, invalidBoundsError + i);
      handles[h][i] = x;
      return 0;
    },
  },
};
