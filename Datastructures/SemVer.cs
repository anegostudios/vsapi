using System;
using System.Collections.Generic;
using System.Text;

namespace ProperVersion
{
	/// <summary>
	///   Implementation of Semantic Verisoning standard, version 2.0.0.
	///   See https://semver.org/ for specifications.
	/// </summary>
	public class SemVer : IComparable<SemVer>, IEquatable<SemVer>
	{
		public int Major { get; }
		public int Minor { get; }
		public int Patch { get; }
		
		public string[] PreReleaseIdentifiers { get; }
		public string[] BuildMetadataIdentifiers { get; }
		
		public string PreRelease => string.Join(".", PreReleaseIdentifiers);
		public string BuildMetadata => string.Join(".", BuildMetadataIdentifiers);
		
		
		public SemVer(int major, int minor, int patch)
			: this(major, minor, patch, new string[0], null,
			                            new string[0], null) {  }
		
		public SemVer(int major, int minor, int patch,
		              string preRelease = "", string buildMetadata = "")
			: this(major, minor, patch,
			       SplitIdentifiers(preRelease), nameof(preRelease),
			       SplitIdentifiers(buildMetadata), nameof(buildMetadata)) {  }
		
		public SemVer(int major, int minor, int patch,
		              string[] preReleaseIdentifiers = null,
		              string[] buildMetadataIdentifiers = null)
			: this(major, minor, patch,
			       preReleaseIdentifiers, nameof(preReleaseIdentifiers),
			       buildMetadataIdentifiers, nameof(buildMetadataIdentifiers)) {  }
		
		private SemVer(int major, int minor, int patch,
		               string[] preReleaseIdentifiers, string preReleaseParamName,
		               string[] buildMetadataIdentifiers, string buildMetadataParamName)
		{
			if (major < 0) throw new ArgumentOutOfRangeException(
				nameof(major), major, "Major value must be 0 or positive");
			if (minor < 0) throw new ArgumentOutOfRangeException(
				nameof(minor), minor, "Minor value must be 0 or positive");
			if (patch < 0) throw new ArgumentOutOfRangeException(
				nameof(patch), patch, "Patch value must be 0 or positive");
			
			if (preReleaseIdentifiers == null)
				preReleaseIdentifiers = new string[0];
			for (var i = 0; i < preReleaseIdentifiers.Length; i++) {
				var ident = preReleaseIdentifiers[i];
				if (ident == null) throw new ArgumentException(
					$"{ preReleaseParamName } contains null element at index { i }", preReleaseParamName);
				if (ident.Length == 0) throw new ArgumentException(
					$"{ preReleaseParamName } contains empty identifier at index { i }", preReleaseParamName);
				if (!IsValidIdentifier(ident)) throw new ArgumentException(
					$"{ preReleaseParamName } contains invalid identifier ('{ ident }') at index { i }", preReleaseParamName);
				if (IsNumericIdent(ident) && (ident[0] == '0') && (ident.Length > 1)) throw new ArgumentException(
					$"{ preReleaseParamName } contains numeric identifier with leading zero(es) at index { i }", preReleaseParamName);
			}
			
			if (buildMetadataIdentifiers == null)
				buildMetadataIdentifiers = new string[0];
			for (var i = 0; i < buildMetadataIdentifiers.Length; i++) {
				var ident = buildMetadataIdentifiers[i];
				if (ident == null) throw new ArgumentException(
					$"{ buildMetadataParamName } contains null element at index { i }", buildMetadataParamName);
				if (ident.Length == 0) throw new ArgumentException(
					$"{ buildMetadataParamName } contains empty identifier at index { i }", buildMetadataParamName);
				if (!IsValidIdentifier(ident)) throw new ArgumentException(
					$"{ buildMetadataParamName } contains invalid identifier ('{ ident }') at index { i }", buildMetadataParamName);
			}
			
			Major = major;
			Minor = minor;
			Patch = patch;
			
			PreReleaseIdentifiers    = preReleaseIdentifiers;
			BuildMetadataIdentifiers = buildMetadataIdentifiers;
		}
		
		
		/// <summary>
		///   Converts the specified string representation of a
		///   semantic version to its <see cref="SemVer"/> equivalent.
		/// </summary>
		/// <exception cref="ArgumentNullException"> Thrown if the specified string is null. </exception>
		/// <exception cref="FormatException"> Thrown if the specified string doesn't contain a proper properly formatted semantic version. </exception>
		public static SemVer Parse(string s)
		{
			TryParse(s, out var result, true);
			return result;
		}

		/// <summary>
		///   Tries to convert the specified string representation of a
		///   semantic version to its <see cref="SemVer"/> equivalent,
		///   returning true if successful.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="result">
		///   When this method returns, contains a valid, non-null SemVer,
		///   If the conversion failed, this is set to the parser's best guess.
		/// </param>
		/// <exception cref="ArgumentNullException"> Thrown if the specified string is null. </exception>
		public static bool TryParse(string s, out SemVer result)
			=> TryParse(s, out result, out var _);

		/// <summary>
		///   Tries to convert the specified string representation of a
		///   semantic version to its <see cref="SemVer"/> equivalent,
		///   returning true if successful.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="result">
		///   When this method returns, contains a valid, non-null SemVer,
		///   If the conversion failed, this is set to the method's best guess.
		/// </param>
		/// <param name="error">
		///   When this method returns, contains the first error describing
		///   why the conversion failed, or null if it succeeded.
		/// </param>
		/// <exception cref="ArgumentNullException"> Thrown if the specified string is null. </exception>
		public static bool TryParse(string s, out SemVer result, out string error)
		{
			error = TryParse(s, out result, false);
			return (error == null);
		}
		
		private static readonly string[] PART_LOOKUP = {
			"MAJOR", "MINOR", "PATCH", "PRE_RELEASE", "BUILD_METADATA" };
		private static string TryParse(string s, out SemVer result, bool throwException)
		{
			if (s == null) throw new ArgumentNullException(nameof(s));
			var sb    = new StringBuilder();
			var error = (string)null;
			
			var mode  = 0; // Current mode, 0 to 2 => expecting the version numbers MAJOR, MINOR and PATCH.
			               //               3 to 4 => expecting PRE_RELEASE and BUILD_METADATA.
			var index = 0;           // Current reading index in the specified string.
			var chr   = (char?)null; // Current character being read, or null if end of string.
			
			// Contain the to-be parsed SemVer information.
			var versions = new int[3];
			var data     = new []{ new List<string>(), new List<string>() };
			
			// Local helper method which will either set error variable or throw
			// a FormatException, using local variables to fill out the message.
			// This helps with not allocating unnecessary objects unless needed.
			void Error(string message, bool nextMode = false)
			{
				// Don't do anything if we already encountered an error before.
				if (error != null) return;
				error = $"Failed parsing version string '{ s }' at index { index }: " + string.Format(message,
					PART_LOOKUP[nextMode ? mode + 1 : mode],
					(chr != null) ? $"'{ chr }'" : "end of string");
				if (throwException) throw new FormatException(error);
			}
			
			for (; index <= s.Length; index++) {
				chr = (index < s.Length) ? s[index] : (char?)null;
				
				// If expecting a MAJOR, MINOR or PATCH version number, ..
				if (mode <= 2) {
					// On digit, write it to the StringBuilder.
					if ((chr >= '0') && (chr <= '9')) {
						sb.Append(chr);
						if ((sb.Length == 2) && (sb[0] == '0'))
							Error("{0} version contains leading zero");
					} else {
						if (sb.Length == 0) // Error if nothing was written to the StringBuilder.
							Error("Expected {0} version number, found {1}");
						else versions[mode] = int.Parse(sb.ToString());
						sb.Clear();
						
						// If the non-digit encountered is a dot, ..
						if (chr == '.') {
							if (mode == 2) // Error if we're already at PATCH number.
								Error("Expected PRE_RELEASE or BUILD_METADATA, found {1}");
							mode++; // Move to the next version number or to PRE_RELEASE.
						} else {
							if (mode != 2) // Error if we're not currently at the PATCH number.
								Error("Expected {0} version, found {1}", true);
							
							if (chr == '-') mode = 3;      // If encountering a hyphen, move to PRE_RELEASE.
							else if (chr == '+') mode = 4; // If encountering a plus, move to BUILD_METADATA.
							else if (chr != null) {
								// Otherwise we found an unexpected character.
								Error("Expected PRE_RELEASE or BUILD_METADATA, found {1}");
								mode = 3; // Move to PRE_RELEASE.
								index--;  // Treat character as part of PRE_RELEASE.
								          // Allows parsing "1.0.0pre2" as "1.0.0-pre2".
							}
						}
					}
				// Otherwise, we're expecting PRE_RELEASE or BUILD_METADATA.
				} else {
					// On valid identifier character, write it to the StringBuilder.
					if (chr.HasValue && IsValidIdentifierChar((char)chr))
						sb.Append(chr);
					// Error if encountering unexpected character.
					else if ((chr != '.') && (chr != null) && // chr is not a dot, the end of the string
					         !((chr == '+') && (mode == 3)))  // or a plus outside of PRE_RELEASE.
						Error("Unexpected character {1} in {0} identifier");
					else {
						if (sb.Length == 0) // Error if nothing was written to the StringBuilder.
							Error("Expected {0} identifier, found {1}");
						else {
							var ident = sb.ToString();
							// Error in PRE_RELEASE if identifier is numeric
							// (just digits) and contains leading zero(es).
							if ((mode == 3) && IsNumericIdent(ident) && (ident[0] == '0') && (ident.Length > 1)) {
								Error("{0} numeric identifier contains leading zero");
								ident = ident.TrimStart('0');
							}
							data[mode - 3].Add(ident);
						}
						sb.Clear();
						// On plus, if currently in PRE_RELEASE, move to BUILD_METADATA.
						if ((chr == '+') && (mode == 3)) mode++;
					}
				}
			}
			
			result = new SemVer(versions[0], versions[1], versions[2],
			                    data[0].ToArray(), data[1].ToArray());
			return error;
		}
		
		
		public override string ToString()
		{
			var sb = new StringBuilder()
				.Append(Major).Append('.')
				.Append(Minor).Append('.')
				.Append(Patch);
			if (PreReleaseIdentifiers.Length > 0)
				sb.Append('-').Append(PreRelease);
			if (BuildMetadataIdentifiers.Length > 0)
				sb.Append('+').Append(BuildMetadata);
			return sb.ToString();
		}
		
		
		public static bool operator ==(SemVer left, SemVer right)
			=> ReferenceEquals(left, right) || (left?.Equals(right) ?? false);
		public static bool operator !=(SemVer left, SemVer right)
			=> !(left == right);
		
		// NOTE: The relational operators behave like lifted Nullable<T> operators:
		//       If either operand, or BOTH operands are null, they return false.
		//       If you want to compare the order of SemVer instances including
		//       null as a valid value, use the Compare or CompareTo methods.
		
		public static bool operator >(SemVer left, SemVer right)
			=> (left != null) && (right != null) && (Compare(left, right) > 0);
		public static bool operator <(SemVer left, SemVer right)
			=> (left != null) && (right != null) && (Compare(left, right) < 0);
		public static bool operator >=(SemVer left, SemVer right)
			=> (left != null) && (right != null) && (Compare(left, right) >= 0);
		public static bool operator <=(SemVer left, SemVer right)
			=> (left != null) && (right != null) && (Compare(left, right) <= 0);
		
		
		public int CompareTo(SemVer other)
			=> Compare(this, other);
		public static int Compare(SemVer left, SemVer right)
		{
			if (ReferenceEquals(left, right)) return 0;
			if (ReferenceEquals(left, null)) return -1;
			if (ReferenceEquals(right, null)) return 1;
			
			var majorDiff = left.Major.CompareTo(right.Major);
			if (majorDiff != 0) return majorDiff;
			var minorDiff = left.Minor.CompareTo(right.Minor);
			if (minorDiff != 0) return minorDiff;
			var patchDiff = left.Patch.CompareTo(right.Patch);
			if (patchDiff != 0) return patchDiff;
			
			// If left has PreRelease and right doesn't, or the other way around,
			// order them by the existance of PreRelease (1.0.0-rc2 < 1.0.0).
			var leftHasPreRelease  = (left.PreReleaseIdentifiers.Length > 0);
			var rightHasPreRelease = (right.PreReleaseIdentifiers.Length > 0);
			if (leftHasPreRelease != rightHasPreRelease)
				return leftHasPreRelease ? -1 : 1;
			
			var minCount = Math.Min(left.PreReleaseIdentifiers.Length,
			                        right.PreReleaseIdentifiers.Length);
			for (var i = 0; i < minCount; i++) {
				var leftIndent  = left.PreReleaseIdentifiers[i];
				var rightIndent = right.PreReleaseIdentifiers[i];
				var leftIdentIsNumeric  = IsNumericIdent(leftIndent);
				var rightIdentIsNumeric = IsNumericIdent(rightIndent);
				
				// If the ident type is different (one is numeric, the other isn't), sort by
				// which one is the numeric one. Numeric identifiers have lower precedence.
				if (leftIdentIsNumeric != rightIdentIsNumeric)
					return leftIdentIsNumeric ? -1 : 1;
				
				var identDiff = (leftIdentIsNumeric)
					// If they're numeric, compare them as numbers.
					? int.Parse(leftIndent).CompareTo(int.Parse(rightIndent))
					// Otherwise compare them lexically in ASCII sort order.
					: string.CompareOrdinal(leftIndent, rightIndent);
				
				// Only return the difference if there is one,
				// otherwise move on to the next identifier.
				if (identDiff != 0) return identDiff;
			}
			
			// When reaching this point, either the amount of identifiers
			// differ between the two versions, or they're truly equivalent.
			return left.PreReleaseIdentifiers.Length - right.PreReleaseIdentifiers.Length;
		}
		
		public bool Equals(SemVer other)
			=> (other != null) &&
			   (Major == other.Major) && (Minor == other.Minor) && (Patch == other.Patch) &&
			   (PreRelease == other.PreRelease) && (BuildMetadata == other.BuildMetadata);
		
		public override bool Equals(object obj)
			=> Equals(obj as SemVer);
		
		public override int GetHashCode()
			=> Major ^ (Minor << 8) ^ (Patch << 16) ^
			   PreRelease.GetHashCode() ^ (BuildMetadata.GetHashCode() << 6);
		
		
		// Various private helper methods...
		
		/// <summary>
		///   Returns whether the specified string contains only valid
		///   identifier characters. That is, only alphanumeric characters
		///   and hyphens, [0-9A-Za-z-]. Does not check for empty identifiers.
		/// </summary>
		private static bool IsValidIdentifier(string ident)
		{
			for (var i = 0; i < ident.Length; i++)
				if (!IsValidIdentifierChar(ident[i]))
					return false;
			return true;
		}
		
		private static bool IsValidIdentifierChar(char chr)
			=> ((chr >= '0') && (chr <= '9')) ||
				((chr >= 'A') && (chr <= 'Z')) ||
				((chr >= 'a') && (chr <= 'z')) ||
				(chr == '-');
		
		/// <summary>
		///   Returns whether the specified string is a
		///   numeric identifier (only contains digits).
		/// </summary>
		private static bool IsNumericIdent(string ident)
		{
			for (var i = 0; i < ident.Length; i++)
				if ((ident[i] < '0') || (ident[i] > '9'))
					return false;
			return true;
		}
		
		/// <summary>
		///   Splits a string into dot-separated identifiers.
		///   Both null and empty strings return an empty array.
		/// </summary>
		private static string[] SplitIdentifiers(string str)
			=> !string.IsNullOrEmpty(str) ? str.Split('.') : new string[0];
	}
}
