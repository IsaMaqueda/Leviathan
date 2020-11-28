# Levithan Compiler - Project make file:

#   Camila Rovirosa A01024192
#   Eduardo Badillo A01020716
#   Isabel Maqueda  A01652906

# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
#

all: leviathan.exe node_modules

node_modules:
	npm init -y
	npm install wabt --save

leviathan.exe: Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs \
	SemanticVisitor.cs SemanticError.cs FunctionTable.cs\
	WATVisitor.cs

	mcs -out:Driver.cs Scanner.cs Token.cs TokenCategory.cs \
	Parser.cs SyntaxError.cs Node.cs SpecificNodes.cs \
	SemanticVisitor.cs SemanticError.cs FunctionTable.cs\
	WATVisitor.cs


clean:
	rm leviathan.exe
	rm -f package.json package-lock.json
	rm -rf node_modules