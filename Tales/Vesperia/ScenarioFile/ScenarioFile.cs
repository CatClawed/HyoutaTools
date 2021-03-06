﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.ScenarioFile {
	public class ScenarioFile {
		public ScenarioFile() { }

		public ScenarioFile( String filename, bool isUtf8 = false ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, isUtf8 ) ) {
					throw new Exception( "Loading ScenarioFile failed!" );
				}
			}
		}

		public ScenarioFile( Stream stream, bool isUtf8 = false ) {
			if ( !LoadFile( stream, isUtf8 ) ) {
				throw new Exception( "Loading ScenarioFile failed!" );
			}
		}

		public List<ScenarioFileEntry> EntryList;
		public string EpisodeID;

		private bool LoadFile( Stream stream, bool isUtf8 ) {
			uint magic = stream.ReadUInt32().SwapEndian();
			uint headerSize = stream.ReadUInt32().SwapEndian();
			stream.ReadUInt32().SwapEndian();
			uint textStart = stream.ReadUInt32().SwapEndian();

			stream.ReadUInt32().SwapEndian();
			stream.ReadUInt32().SwapEndian();
			uint textLength = stream.ReadUInt32().SwapEndian();
			stream.ReadUInt32().SwapEndian();

			EntryList = FindText( stream, textStart, isUtf8 );

			return true;
		}

		private static List<ScenarioFileEntry> FindText( Stream stream, uint textStart, bool isUtf8 ) {
			var list = new List<ScenarioFileEntry>();

			while ( stream.Position < textStart ) {
				uint identifyingBytes = stream.ReadUInt32();
				if ( identifyingBytes == 0x18000C04 ) {
					uint pos = (uint)stream.Position;

					uint pointerToText = stream.ReadUInt32().SwapEndian() + textStart;
					
					stream.Position = pointerToText;
					stream.ReadUInt32().SwapEndian();
					stream.ReadUInt32().SwapEndian();
					uint jpNamePtr = stream.ReadUInt32().SwapEndian();
					uint jpTextPtr = stream.ReadUInt32().SwapEndian();
					uint enNamePtr = stream.ReadUInt32().SwapEndian();
					uint enTextPtr = stream.ReadUInt32().SwapEndian();
					stream.Position = jpNamePtr + textStart; string jpName = isUtf8 ? stream.ReadUTF8Nullterm() : stream.ReadShiftJisNullterm();
					stream.Position = jpTextPtr + textStart; string jpText = isUtf8 ? stream.ReadUTF8Nullterm() : stream.ReadShiftJisNullterm();
					stream.Position = enNamePtr + textStart; string enName = isUtf8 ? stream.ReadUTF8Nullterm() : stream.ReadShiftJisNullterm();
					stream.Position = enTextPtr + textStart; string enText = isUtf8 ? stream.ReadUTF8Nullterm() : stream.ReadShiftJisNullterm();

					var entry = new ScenarioFileEntry() { Pointer = pos, JpName = jpName, EnName = enName, JpText = jpText, EnText = enText };
					list.Add( entry );

					stream.Position = pos + 4;
				}
			}

			return list;
		}

		public ScenarioFile CloneShallow() {
			ScenarioFile clone = new ScenarioFile();
			
			clone.EntryList = new List<ScenarioFileEntry>( this.EntryList.Count );
			clone.EntryList.AddRange( this.EntryList );
			clone.EpisodeID = EpisodeID;

			return clone;
		}

		public override string ToString() {
			return EpisodeID + ": " + EntryList.Count + " entries";
		}
	}
}
