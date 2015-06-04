// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Units
{
	internal class Cruiser : BaseUnit
	{
		public Cruiser() : base(8, 6, 6, 6)
		{
			Class = UnitClass.Water;
			Type = Unit.Cruiser;
			Name = "Cruiser";
			RequiredTech = null;
			ObsoleteTech = null;
		}
	}
}