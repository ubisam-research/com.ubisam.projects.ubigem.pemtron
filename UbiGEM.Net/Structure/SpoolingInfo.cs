using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGEM.Net.Structure
{
    /// <summary>
    /// Spooling 대상 Message 정보입니다.
    /// </summary>
    public class SpoolingCollection
    {
        private const int ALL_FUNCTION = -1;

        private readonly Dictionary<int, List<int>> _items;
        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SpoolingCollection()
        {
            this._items = new Dictionary<int, List<int>>();
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            return string.Format("Item Count={0}", this._items.Count);
        }

        /// <summary>
        /// Spooling할 Message 정보를 추가합니다.
        /// </summary>
        /// <param name="stream">추가할 Steam Number입니다.</param>
        public void Add(int stream)
        {
            if (stream != 1)
            {
                if (this._items.ContainsKey(stream) == false)
                    this._items[stream] = new List<int>();

                this._items[stream].Add(ALL_FUNCTION);
            }
        }

        /// <summary>
        /// Spooling할 Message 정보를 추가합니다.
        /// </summary>
        /// <param name="stream">추가할 Steam Number입니다.</param>
        /// <param name="function">추가할 Function Number입니다.</param>
        public void Add(int stream, int function)
        {
            if (stream != 1)
            {
                if (this._items.ContainsKey(stream) == false)
                    this._items[stream] = new List<int>();

                this._items[stream].Add(function);
            }
        }

        /// <summary>
        /// Variable 정보를 삭제합니다.
        /// </summary>
        public void Remove()
        {
            this._items.Clear();
        }

        /// <summary>
        /// Variable 정보를 삭제합니다.
        /// </summary>
        /// <param name="stream">삭제할 Stream Number입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Remove(int stream)
        {
            return this._items.Remove(stream);
        }

        /// <summary>
        /// Variable 정보를 삭제합니다.
        /// </summary>
        /// <param name="stream">삭제할 Stream Number입니다.</param>
        /// <param name="function">삭제할 Function Number입니다.</param>
        /// <returns>요소를 성공적으로 찾아서 제거한 경우 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Remove(int stream, int function)
        {
            bool result;

            result = false;

            if (this._items.ContainsKey(stream) == true)
            {
                this._items[stream].Remove(function);

                if (this._items[stream].Count <= 0)
                {
                    result = this._items.Remove(stream);
                }
            }

            return result;
        }

        /// <summary>
        /// 지정된 Spool 대상 정보가 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="stream">검사할 Stream Number입니다.</param>
        /// <param name="function">검사할 Function Number입니다.</param>
        /// <returns>포함되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        public bool Exist(int stream, int function)
        {
            bool result;

            if (this._items.ContainsKey(stream) == true)
            {
                if (this._items[stream].Count == 1 && this._items[stream][0] == ALL_FUNCTION)
                {
                    result = true;
                }
                else
                {
                    result = this._items[stream].Exists(t => t == function);
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}