using System;
using System.Collections.Generic;
using System.Linq;
using UbiCom.Net.Structure;

namespace UbiCom.Net.Automata.SECS2
{
    public class Automata
    {
        #region [Constructors and Descturctors]
        public Automata()
        {
        }
        #endregion
        #region [Public Methods]
        public SECSMessage MakeSECSMessageUsingAutomata(string name, SECSMessageDirection direction, int stream, int function, bool isWait, string data, out string invalidLine, out string errorText)
        {
            SECSMessage message;
            List<LogBasicStructure> logStructure;
            string trimmedData;

            message = null;
            invalidLine = string.Empty;
            errorText = string.Empty;

            if (string.IsNullOrEmpty(data) == false)
            {
                trimmedData = data.Trim();

                if (CheckPairsInData(trimmedData, out invalidLine, out errorText) == true)
                {
                    logStructure = RunAutomata(trimmedData, out invalidLine, out errorText);

                    if (string.IsNullOrEmpty(errorText) == true)
                    {
                        message = new SECSMessage
                        {
                            Name = name,
                            Direction = direction,
                            Stream = stream,
                            Function = function,
                            WaitBit = isWait
                        };

                        BuildSECSMessage(message, logStructure, out invalidLine, out errorText);
                    }
                }
            }
            else
            {
                message = new SECSMessage
                {
                    Name = name,
                    Direction = direction,
                    Stream = stream,
                    Function = function,
                    WaitBit = isWait
                };
            }

            return message;
        }
        #endregion
        #region [Private Methods]
        private void BuildSECSMessage(SECSMessage message, List<LogBasicStructure> logStructure, out string invalidLine, out string errorText)
        {
            string itemName;
            bool isFixed;
            string itemData;
            dynamic data;

            invalidLine = string.Empty;
            errorText = string.Empty;

            foreach (LogBasicStructure structure in logStructure)
            {
                if (Enum.TryParse(structure.Format, out SECSItemFormat itemFormat) == false)
                {
                    errorText = string.Format(" invalid format: {0}", structure.Format);
                    invalidLine = structure.LineString.Trim();
                }
                else
                {
                    if (itemFormat == SECSItemFormat.X || itemFormat == SECSItemFormat.None)
                    {
                        errorText = string.Format(" invalid format: {0}", structure.Format);
                        invalidLine = structure.LineString.Trim();
                    }

                    if (int.TryParse(structure.Length, out int itemLength) == false)
                    {
                        errorText = string.Format(" invalid length: {0}", structure.Length);
                        invalidLine = structure.LineString.Trim();
                    }
                    else
                    {
                        itemName = string.Empty;

                        if (structure.Name != null)
                        {
                            itemName = structure.Name;
                        }

                        isFixed = true;

                        if (itemFormat == SECSItemFormat.L || itemLength == 0 || structure.Value == null)
                        {
                            itemData = string.Empty;
                        }
                        else
                        {
                            itemData = structure.Value;
                        }

                        if (itemFormat == SECSItemFormat.L)
                        {
                            message.Body.Add(itemName, itemFormat, itemLength, isFixed, null);
                        }
                        else if (itemFormat == SECSItemFormat.A || itemFormat == SECSItemFormat.J)
                        {
                            if (itemData.Length > itemLength)
                            {
                                itemData = itemData.Substring(0, itemLength);
                            }

                            message.Body.Add(itemName, itemFormat, itemLength, isFixed, itemData);
                        }
                        else
                        {
                            if (itemLength == 0)
                            {
                                message.Body.Add(itemName, itemFormat, 0, isFixed, string.Empty);
                            }
                            else if (itemLength == 1)
                            {
                                data = this.ConvertValue(itemFormat, itemData);

                                if (data == null)
                                {
                                    invalidLine = structure.LineString.Trim();
                                    errorText = string.Format("invalid data: {0}", itemData);
                                    break;
                                }
                                else
                                {
                                    message.Body.Add(itemName, itemFormat, 1, isFixed, data);
                                }
                            }
                            else
                            {
                                if (this.AddListToMessage(message, itemFormat, itemName, itemLength, isFixed, itemData, out errorText) == false)
                                {
                                    invalidLine = structure.LineString.Trim();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private List<LogBasicStructure> RunAutomata(string data, out string invalidLine, out string errorText)
        {
            List<LogBasicStructure> logStructure;
            Stack<int> itemCountStack;
            Stack<int> listIndexStack;

            int rootItemCount;
            int depth;

            string processData;
            List<int> splitIndexCollection;
            List<string> splitLineCollection;
            int lineStartIndex;
            int lineEndIndex;
            int lineLength;

            int processDataLength;

            int tempLength;

            char c;

            string formatString;
            string countString;
            string dataString;
            string nameString;

            splitIndexCollection = new List<int>();
            splitLineCollection = new List<string>();

            AutomataState currentState;
            AutomataState newState;

            itemCountStack = new Stack<int>();
            listIndexStack = new Stack<int>();
            logStructure = new List<LogBasicStructure>();

            invalidLine = string.Empty;
            errorText = string.Empty;

            formatString = string.Empty;
            countString = string.Empty;
            dataString = string.Empty;
            nameString = string.Empty;

            if (string.IsNullOrEmpty(data) == false)
            {
                processData = data.Replace("\r", string.Empty);

                lineStartIndex = 0;
                lineEndIndex = processData.IndexOf('\n');
                lineLength = lineEndIndex - lineStartIndex;

                while (true)
                {
                    if (lineLength <= 0)
                    {
                        splitIndexCollection.Add(lineStartIndex);
                        splitLineCollection.Add(processData.Substring(lineStartIndex));
                        break;
                    }

                    splitIndexCollection.Add(lineStartIndex);
                    splitLineCollection.Add(processData.Substring(lineStartIndex, lineLength));

                    lineStartIndex = lineEndIndex + 1;
                    lineEndIndex = processData.IndexOf('\n', lineStartIndex);
                    lineLength = lineEndIndex - lineStartIndex;
                }

                processDataLength = processData.Length;

                currentState = AutomataState.Start;

                rootItemCount = 0;
                depth = 0;

                for (int i = 0; i < processDataLength; i++)
                {
                    if (currentState != AutomataState.Error)
                    {
                        c = processData[i];

                        switch (currentState)
                        {
                            #region AutomataState.InItemEnd
                            case AutomataState.InItemEnd:
                                {
                                    CheckAutomataStateInItemEnd(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after '>'";
                                    }
                                    else
                                    {
                                        if (itemCountStack.Count == 0)
                                        {
                                            if (listIndexStack.Count == 0)
                                            {
                                                invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                errorText = " adding 0 length list item";
                                            }
                                            else
                                            {
                                                if (depth == 0)
                                                {
                                                    if (rootItemCount > 0)
                                                    {
                                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                        errorText = "adding item to root";
                                                        newState = AutomataState.Error;
                                                    }

                                                    rootItemCount++;
                                                }
                                                else
                                                {
                                                    tempLength = listIndexStack.Pop();
                                                    invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, tempLength)];
                                                    errorText = " adding 0 length list item";
                                                    newState = AutomataState.Error;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tempLength = itemCountStack.Pop();

                                            if (tempLength == 0)
                                            {
                                                if (listIndexStack.Count == 0)
                                                {
                                                    invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                    errorText = " adding 0 length list item";
                                                }
                                                else
                                                {
                                                    if (depth == 0)
                                                    {
                                                        if (rootItemCount > 0)
                                                        {
                                                            invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                            errorText = "adding item to root";
                                                        }

                                                        rootItemCount++;
                                                    }
                                                    else
                                                    {
                                                        tempLength = listIndexStack.Pop();
                                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, tempLength)];
                                                        errorText = " adding 0 length list item";
                                                    }
                                                }

                                                newState = AutomataState.Error;
                                            }
                                            else
                                            {
                                                if (tempLength > 1)
                                                {
                                                    itemCountStack.Push(tempLength - 1);
                                                }
                                                else
                                                {
                                                    depth--;
                                                }
                                            }
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InAfterNameString
                            case AutomataState.InAfterNameString:
                                {
                                    CheckAutomataStateInAfterNameString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after name";
                                    }
                                    else
                                    {
                                        if (newState == AutomataState.InItemEnd)
                                        {
                                            if (depth == 0)
                                            {
                                                if (rootItemCount > 0)
                                                {
                                                    invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                    errorText = "adding item to root";
                                                    newState = AutomataState.Error;
                                                }

                                                rootItemCount++;
                                            }

                                            logStructure.Add(new LogBasicStructure()
                                            {
                                                LineString = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)],
                                                Format = formatString,
                                                Length = countString,
                                                Value = dataString,
                                                Name = nameString,
                                            });
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InNameString
                            case AutomataState.InNameString:
                                {
                                    CheckAutomataStateInNameString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character in name";
                                    }
                                    else
                                    {
                                        if (currentState == newState)
                                        {
                                            nameString += c.ToString();
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InBeforeNameString
                            case AutomataState.InBeforeNameString:
                                {
                                    CheckAutomataStateInBeforeNameString(currentState, c, out newState);
                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after value";
                                    }
                                    else
                                    {
                                        if (newState == AutomataState.InItemEnd)
                                        {
                                            if (depth == 0)
                                            {
                                                if (rootItemCount > 0)
                                                {
                                                    invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                    errorText = "adding item to root";
                                                    newState = AutomataState.Error;
                                                }

                                                rootItemCount++;
                                            }

                                            logStructure.Add(new LogBasicStructure()
                                            {
                                                LineString = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)],
                                                Format = formatString,
                                                Length = countString,
                                                Value = dataString,
                                                Name = nameString,
                                            });
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InValueString
                            case AutomataState.InValueString:
                                {
                                    CheckAutomataStateInValueString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after in value";
                                    }
                                    else
                                    {
                                        if (currentState == newState)
                                        {
                                            dataString += c.ToString();
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InCountString
                            case AutomataState.InCountString:
                                {
                                    CheckAutomataStateInCountString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character in count";
                                    }
                                    else
                                    {
                                        if (currentState == newState)
                                        {
                                            countString += c.ToString();
                                        }
                                        else
                                        {
                                            if (newState == AutomataState.InItemEnd)
                                            {
                                                if (depth == 0)
                                                {
                                                    if (rootItemCount > 0)
                                                    {
                                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                        errorText = "adding item to root";
                                                    }

                                                    rootItemCount++;
                                                }

                                                logStructure.Add(new LogBasicStructure()
                                                {
                                                    LineString = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)],
                                                    Format = formatString,
                                                    Length = countString,
                                                    Value = dataString,
                                                    Name = nameString,
                                                });
                                            }
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InFormatString
                            case AutomataState.InFormatString:
                                {
                                    CheckAutomataStateInFormatString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after '<'";
                                    }
                                    else
                                    {
                                        if (newState == AutomataState.InFormatString || newState == AutomataState.InListFormatString)
                                        {
                                            formatString += c.ToString();
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InAfterListNameString
                            case AutomataState.InAfterListNameString:
                                {
                                    CheckAutomataStateInAfterListNameString(currentState, c, out newState);
                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after name";
                                    }
                                    else
                                    {
                                        if (currentState != newState)
                                        {
                                            if (Enum.TryParse<SECSItemFormat>(formatString.Trim(), out _) == false)
                                            {
                                                invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                errorText = "invalid character in format";

                                                newState = AutomataState.Error;
                                            }

                                            if (int.TryParse(countString.Trim(), out tempLength) == false)
                                            {
                                                invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                errorText = "invalid character in count";

                                                newState = AutomataState.Error;
                                            }

                                            if (newState == AutomataState.Start || newState == AutomataState.InItemEnd)
                                            {
                                                if (tempLength > 0)
                                                {
                                                    itemCountStack.Push(tempLength);
                                                    listIndexStack.Push(i);
                                                    depth++;
                                                }

                                                logStructure.Add(new LogBasicStructure()
                                                {
                                                    LineString = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)],
                                                    Format = formatString,
                                                    Length = countString,
                                                    Value = dataString,
                                                    Name = nameString,
                                                });
                                            }
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InListNameString
                            case AutomataState.InListNameString:
                                {
                                    CheckAutomataStateInListNameString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character in name";
                                    }
                                    else
                                    {
                                        if (currentState == newState)
                                        {
                                            nameString += c.ToString();
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InListCountString
                            case AutomataState.InListCountString:
                                {
                                    CheckAutomataStateInListCountString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character in count";
                                    }
                                    else
                                    {
                                        if (currentState == newState)
                                        {
                                            countString += c.ToString();
                                        }
                                        else
                                        {
                                            if (Enum.TryParse<SECSItemFormat>(formatString.Trim(), out _) == false)
                                            {
                                                invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                errorText = "invalid character in format";

                                                newState = AutomataState.Error;
                                            }

                                            if (int.TryParse(countString.Trim(), out tempLength) == false)
                                            {
                                                invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                                errorText = "invalid character in count";

                                                newState = AutomataState.Error;
                                            }

                                            if (newState == AutomataState.Start || newState == AutomataState.InItemEnd)
                                            {
                                                if (tempLength > 0)
                                                {
                                                    itemCountStack.Push(tempLength);
                                                    listIndexStack.Push(i);
                                                    depth++;
                                                }

                                                logStructure.Add(new LogBasicStructure()
                                                {
                                                    LineString = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)],
                                                    Format = formatString,
                                                    Length = countString,
                                                    Value = dataString,
                                                    Name = nameString,
                                                });
                                            }
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.InListFormatString
                            case AutomataState.InListFormatString:
                                {
                                    CheckAutomataStateInListFormatString(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character after '<'";
                                    }
                                    else
                                    {
                                        if (currentState == newState)
                                        {
                                            formatString += c.ToString();
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                            #endregion
                            #region AutomataState.Start
                            case AutomataState.Start:
                                {
                                    CheckAutomataStateStart(currentState, c, out newState);

                                    if (newState == AutomataState.Error)
                                    {
                                        invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, i)];
                                        errorText = "invalid character before '<'";
                                    }
                                    else
                                    {
                                        if (currentState != newState)
                                        {
                                            formatString = string.Empty;
                                            countString = string.Empty;
                                            dataString = string.Empty;
                                            nameString = string.Empty;
                                        }

                                        currentState = newState;
                                    }
                                }

                                break;
                                #endregion
                        }
                    }
                }
            }

            if (itemCountStack.Count != 0)
            {
                if (string.IsNullOrEmpty(errorText) == true)
                {
                    tempLength = listIndexStack.Pop();
                    invalidLine = splitLineCollection[FindInvalidIndex(splitIndexCollection, tempLength)];
                    errorText = " list item count mismatch";
                }
            }

            return logStructure;
        }
        private int FindInvalidIndex(List<int> splitIndexCollection, int charIndex)
        {
            int result;
            int beforeIndex;

            if (splitIndexCollection == null || splitIndexCollection.Count == 0)
            {
                result = 0;
            }
            else
            {
                result = 0;
                beforeIndex = 0;

                foreach (int lineStartIndex in splitIndexCollection)
                {
                    if (charIndex < lineStartIndex)
                    {
                        break;
                    }

                    beforeIndex = result;
                    result++;
                }

                result = beforeIndex;
            }

            return result;
        }
        private void CheckAutomataStateStart(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '<')
            {
                newState = AutomataState.InFormatString;
            }
            else if (c == '>')
            {
                newState = AutomataState.InItemEnd;
            }
            else
            {
                if (c != ' ' && c != '\n')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInListFormatString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == ',')
            {
                newState = AutomataState.InListCountString;
            }
            else
            {
                if (c == '\n')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInListCountString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '>')
            {
                newState = AutomataState.InItemEnd;
            }
            else if (c == '\n')
            {
                newState = AutomataState.Start;
            }
            else if (c == '[')
            {
                newState = AutomataState.InListNameString;
            }
            else if (c != ' ')
            {
                if (char.IsDigit(c) == false)
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInListNameString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '\n')
            {
                newState = AutomataState.Error;
            }
            else if (c == ']')
            {
                newState = AutomataState.InAfterListNameString;
            }
        }
        private void CheckAutomataStateInAfterListNameString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '\n')
            {
                newState = AutomataState.Start;
            }
            else if (c == '>')
            {
                newState = AutomataState.InItemEnd;
            }
            else
            {
                if (c != ' ')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInFormatString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == ',')
            {
                newState = AutomataState.InCountString;
            }

            if (c == 'L')
            {
                newState = AutomataState.InListFormatString;
            }
            else
            {
                if (c == '\n')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInCountString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '>')
            {
                newState = AutomataState.InItemEnd;
            }
            else if (c == '\n')
            {
                newState = AutomataState.Error;
            }
            else if (c == '[')
            {
                newState = AutomataState.InNameString;
            }
            else if (c == '\'')
            {
                newState = AutomataState.InValueString;
            }
            else if (c != ' ')
            {
                if (char.IsDigit(c) == false)
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInValueString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '\'')
            {
                newState = AutomataState.InBeforeNameString;
            }
            else if (c == '\n')
            {
                newState = AutomataState.Error;
            }
        }
        private void CheckAutomataStateInBeforeNameString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '>')
            {
                newState = AutomataState.InItemEnd;
            }
            else if (c == '[')
            {
                newState = AutomataState.InNameString;
            }
            else
            {
                if (c != ' ')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInNameString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '\n')
            {
                newState = AutomataState.Error;
            }
            else if (c == ']')
            {
                newState = AutomataState.InAfterNameString;
            }
        }
        private void CheckAutomataStateInAfterNameString(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '>')
            {
                newState = AutomataState.InItemEnd;
            }
            else
            {
                if (c != ' ')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private void CheckAutomataStateInItemEnd(AutomataState currentState, char c, out AutomataState newState)
        {
            newState = currentState;

            if (c == '\n' || c == '<')
            {
                newState = AutomataState.Start;
            }
            else
            {
                if (c != ' ')
                {
                    newState = AutomataState.Error;
                }
            }
        }
        private bool CheckPairsInData(string data, out string invalidLine, out string errorText)
        {
            bool result;
            string[] splitData;
            int apostropheCount;  // '
            int openBraceCount;  // [ or <
            int closeBraceCount;  // ] or <
            int firstApostropheIndex;
            int secondfirstApostropheIndex;
            int openBraceIndex;
            int closeBraceIndex;
            string trimmedLine;

            result = true;
            invalidLine = string.Empty;
            errorText = string.Empty;

            if (string.IsNullOrEmpty(data) == false)
            {
                openBraceCount = data.Count(t => t == '<');
                closeBraceCount = data.Count(t => t == '>');

                if (openBraceCount != closeBraceCount)
                {
                    result = false;
                    invalidLine = string.Empty;
                    errorText = " < count and > count mismatch";
                }
                else
                {
                    splitData = data.Replace("\r", string.Empty).Split('\n').Where(t => t.Length > 0).ToArray();

                    foreach (string line in splitData)
                    {
                        trimmedLine = line.Trim();

                        // check '' pair
                        apostropheCount = trimmedLine.Count(t => t == '\'');

                        if (apostropheCount != 0 && apostropheCount != 2)
                        {
                            result = false;
                            invalidLine = trimmedLine;
                            errorText = " invalid \' pair";
                        }
                        else
                        {
                            // check [] pair
                            openBraceCount = trimmedLine.Count(t => t == '[');
                            closeBraceCount = trimmedLine.Count(t => t == ']');

                            if (openBraceCount > 0 && closeBraceCount > 0)
                            {
                                if (openBraceCount != closeBraceCount)
                                {
                                    result = false;
                                    invalidLine = trimmedLine;
                                    errorText = " invalid [] pair";
                                }
                                else
                                {
                                    if (openBraceCount > 1 || closeBraceCount > 1)
                                    {
                                        result = false;
                                        invalidLine = trimmedLine;
                                        errorText = " use only 1 [] pair";
                                    }
                                    else if (apostropheCount > 0)
                                    {
                                        firstApostropheIndex = trimmedLine.IndexOf('\'');
                                        secondfirstApostropheIndex = trimmedLine.LastIndexOf('\'');
                                        openBraceIndex = trimmedLine.IndexOf('[');
                                        closeBraceIndex = trimmedLine.IndexOf(']');

                                        if (closeBraceIndex < openBraceIndex)
                                        {
                                            result = false;
                                            invalidLine = trimmedLine;
                                            errorText = " order of [ and ]";
                                        }
                                        else if (openBraceIndex < secondfirstApostropheIndex)
                                        {
                                            result = false;
                                            invalidLine = trimmedLine;
                                            errorText = " order if \' and [";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
        private dynamic ConvertValue(SECSItemFormat format, string value)
        {
            dynamic result;

            if (value == null)
            {
                result = string.Empty;
            }
            else
            {
                switch (format)
                {
                    case SECSItemFormat.L:
                        result = null;
                        break;
                    case SECSItemFormat.Boolean:
                        if (bool.TryParse(value, out bool boolValue) == true)
                        {
                            result = boolValue;
                        }
                        else
                        {
                            if (value == "1")
                            {
                                result = true;
                            }
                            else if (value == "0")
                            {
                                result = false;
                            }
                            else
                            {
                                result = null;
                            }
                        }

                        break;
                    case SECSItemFormat.B:
                    case SECSItemFormat.U1:
                        if (byte.TryParse(value, out byte byteValue) == true)
                        {
                            result = byteValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.U2:
                        if (ushort.TryParse(value, out ushort ushortValue) == true)
                        {
                            result = ushortValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.U4:
                        if (uint.TryParse(value, out uint uintValue) == true)
                        {
                            result = uintValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.U8:
                        if (ulong.TryParse(value, out ulong ulongValue) == true)
                        {
                            result = ulongValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I1:
                        if (sbyte.TryParse(value, out sbyte sbyteValue) == true)
                        {
                            result = sbyteValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I2:
                        if (short.TryParse(value, out short shortValue) == true)
                        {
                            result = shortValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I4:
                        if (int.TryParse(value, out int intValue) == true)
                        {
                            result = intValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.I8:
                        if (long.TryParse(value, out long longValue) == true)
                        {
                            result = longValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.F4:
                        if (float.TryParse(value, out float floatValue) == true)
                        {
                            result = floatValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.F8:
                        if (double.TryParse(value, out double doubleValue) == true)
                        {
                            result = doubleValue;
                        }
                        else
                        {
                            result = null;
                        }

                        break;
                    case SECSItemFormat.A:
                    case SECSItemFormat.J:
                    default:
                        result = value;
                        break;
                }
            }

            return result;
        }
        private bool AddListToMessage(SECSMessage message, SECSItemFormat format, string name, int count, bool isFixed, string dataString, out string errorText)
        {
            bool result;
            List<sbyte> sbyteList;
            List<byte> byteList;
            List<short> shortList;
            List<ushort> ushortList;
            List<int> intList;
            List<uint> uintList;
            List<long> longList;
            List<ulong> ulongList;
            List<float> floatList;
            List<double> doubleList;
            List<bool> boolList;

            errorText = string.Empty;
            result = true;

            switch (format)
            {
                case SECSItemFormat.Boolean:
                    boolList = MakeList<bool>(format, count, isFixed, dataString);

                    if (boolList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, boolList.Count, boolList.ToArray());
                    }

                    break;
                case SECSItemFormat.B:
                case SECSItemFormat.U1:
                    byteList = MakeList<byte>(format, count, isFixed, dataString);

                    if (byteList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, byteList.Count, byteList.ToArray());
                    }

                    break;
                case SECSItemFormat.U2:
                    ushortList = MakeList<ushort>(format, count, isFixed, dataString);

                    if (ushortList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, ushortList.Count, ushortList.ToArray());
                    }

                    break;
                case SECSItemFormat.U4:
                    uintList = MakeList<uint>(format, count, isFixed, dataString);

                    if (uintList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, uintList.Count, uintList.ToArray());
                    }

                    break;
                case SECSItemFormat.U8:
                    ulongList = MakeList<ulong>(format, count, isFixed, dataString);

                    if (ulongList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, ulongList.Count, ulongList.ToArray());
                    }

                    break;
                case SECSItemFormat.I1:
                    sbyteList = MakeList<sbyte>(format, count, isFixed, dataString);

                    if (sbyteList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, sbyteList.Count, sbyteList.ToArray());
                    }

                    break;
                case SECSItemFormat.I2:
                    shortList = MakeList<short>(format, count, isFixed, dataString);

                    if (shortList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, shortList.Count, shortList.ToArray());
                    }

                    break;
                case SECSItemFormat.I4:
                    intList = MakeList<int>(format, count, isFixed, dataString);

                    if (intList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, intList.Count, intList.ToArray());
                    }

                    break;
                case SECSItemFormat.I8:
                    longList = MakeList<long>(format, count, isFixed, dataString);

                    if (longList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, longList.Count, longList.ToArray());
                    }

                    break;
                case SECSItemFormat.F4:
                    floatList = MakeList<float>(format, count, isFixed, dataString);

                    if (floatList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, floatList.Count, floatList.ToArray());
                    }

                    break;
                case SECSItemFormat.F8:
                    doubleList = MakeList<double>(format, count, isFixed, dataString);

                    if (doubleList == null)
                    {
                        if (count == 0)
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, data: {1}", format.ToString(), dataString);
                        }
                        else
                        {
                            errorText = string.Format(" format and data mismatch. \n\n format: {0}, count: {1}, data: {2}", format.ToString(), count, dataString);
                        }
                    }
                    else
                    {
                        message.Body.Add(name, format, doubleList.Count, doubleList.ToArray());
                    }

                    break;
                default:
                    result = false;
                    break;
            }

            if (string.IsNullOrEmpty(errorText) == false)
            {
                result = false;
            }

            return result;
        }
        private List<T> MakeList<T>(SECSItemFormat format, int count, bool isFixed, string data)
        {
            List<T> list;
            string[] splitData;
            dynamic converted;
            T value;

            list = null;

            splitData = data.Split(' ');

            if (count == 0 || (isFixed == true && count == splitData.Length) || (isFixed == false && count <= splitData.Length))
            {
                list = new List<T>();

                foreach (string temp in splitData)
                {
                    converted = ConvertValue(format, temp);

                    if (converted == null)
                    {
                        list = null;
                        break;
                    }
                    else
                    {
                        value = converted;
                        list.Add(value);
                    }
                }
            }

            return list;
        }
        #endregion
    }
}
