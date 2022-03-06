﻿using System;
using System.Collections.Generic;

namespace BetterJSON
{
	public class JSONArray
	{
		private List<JSONValue> values;

		public int Length { get { return values.Count; } }

		public JSONArray(string jsonString)
		{
			values = new List<JSONValue>();

			Parse(jsonString);
		}

		public JSONValue this[int index]
		{
			get { return values[index]; }
			set { values[index] = value; }
		}

		private void Parse(string jsonString)
		{
			if(string.IsNullOrEmpty(jsonString)) { return; } //Empty
			if(!(jsonString[0] == '[' && jsonString[jsonString.Length - 1] == ']')) { return; } //Invalid format
			if(jsonString.Length <= 2) { return; } //No contents

			string contentString = jsonString.Substring(1, jsonString.Length - 2).Trim();
			int length = contentString.Length;

			int valueStartIndex = 0;
			int valueEndIndex = 0;
			string valueString = null;

			while(length > 0)
			{
				switch(contentString[0])
				{
					case '{':
						DetermineRange(contentString, ref valueStartIndex, ref valueEndIndex);
						valueString = contentString.Substring(valueStartIndex, (valueEndIndex - valueStartIndex) + 1);
						values.Add(new ObjectValue(valueString));
						break;
					case '\"':
						valueStartIndex = 0;
						valueEndIndex = contentString.IndexOf("\",") - 1;
						if(valueEndIndex < 0)
						{
							if(contentString[length - 1] == '\"')
							{
								valueEndIndex = length - 1;
							}

							if(valueEndIndex == -1) { break; } //Invalid format
						}

						valueString = contentString.Substring(valueStartIndex, (valueEndIndex - valueStartIndex) + 1);
						values.Add(new StringValue(valueString));
						break;
					default:
						valueStartIndex = 0;
						valueEndIndex = contentString.IndexOf(',') - 1;
						if(valueEndIndex < 0)
						{
							valueEndIndex = contentString.IndexOf('}') - 1;
						}
						if(valueEndIndex < 0)
						{
							valueEndIndex = contentString.Length - 1;
						}

						valueString = contentString.Substring(valueStartIndex, (valueEndIndex - valueStartIndex) + 1);
						if(valueString.EndsWith("}"))
						{
							valueString = valueString.Substring(0, valueString.Length - 1);
						}

						if(valueString.Equals("false", StringComparison.OrdinalIgnoreCase) || valueString.Equals("true", StringComparison.OrdinalIgnoreCase))
						{
							values.Add(new BooleanValue(valueString));
						}
						else if(valueString.Contains("."))
						{
							values.Add(new DoubleValue(valueString));
						}
						else
						{
							values.Add(new IntegerValue(valueString));
						}
						break;
				}

				if(valueEndIndex + 1 < length)
				{
					contentString = contentString.Substring(valueEndIndex + 1);
					length = contentString.Length;

					if(length > 0 && contentString[0] == ',')
					{
						contentString = contentString.Substring(1);
						length = contentString.Length;
					}
				}
				else
				{
					break;
				}
			}
		}

		public void DetermineRange(string contentString, ref int startIndex, ref int endIndex)
		{
			startIndex = 0;
			int depth = 0;

			int length = contentString.Length;
			for(int i = 0; i < length; i++)
			{
				char c = contentString[i];
				if(c == '[' || c == '{')
				{
					depth++;
				}
				else if(c == ']' || c == '}')
				{
					depth--;
					if(depth == 0)
					{
						endIndex = i;
						break;
					}
				}
			}
		}
	}
}
