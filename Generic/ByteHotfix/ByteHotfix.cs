﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HyoutaTools.Generic.ByteHotfix {
	class ByteHotfix {
		public static int Execute( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: ByteHotfix [filename] [location-byte] [location-byte] etc." );
				Console.WriteLine( "example: ByteHotfix 325.new 3A9A3-94 3AA72-A4 3AA73-32 3AB53-51" );
				return -1;
			}

			/*
			args = new string[] { @"c:\#gn_chat\scenario.dat.ext.ex\325.new" ,
				"3A9A3-94", "3AA72-A4", "3AA73-32", "3AB53-51" };
			 */

			try {
				string inFilename = args[0];

				using ( var fi = new System.IO.FileStream( inFilename, System.IO.FileMode.Open ) ) {
					for ( int i = 1; i < args.Count; i++ ) {
						String[] v = args[i].Split( new char[] { '-' } );
						int location = int.Parse( v[0], NumberStyles.AllowHexSpecifier );
						byte value = byte.Parse( v[1], NumberStyles.AllowHexSpecifier );
						fi.Position = location;
						fi.WriteByte( value );
					}
					fi.Close();
				}

				return 0;

			} catch ( Exception ex ) {
				Console.WriteLine( "Exception: " + ex.Message );
				return -1;
			}
		}
	}
}
