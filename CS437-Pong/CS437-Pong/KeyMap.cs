using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace CS437_Pong
{
    // https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-mapvirtualkeya
    /// <summary>
    /// A wrapper around the MapVirtualKey API of Windows.  This is used to convert scan codes (region independent key
    /// layout) to the currently active layout.  Useful for users of Workman, Dvorak, AZERTY, etc.
    /// </summary>
    class KeyMap
    {
        /// <summary>
        /// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a
        /// virtual-key code.
        /// </summary>
        /// <param name="uCode">The virtual key code or scan code for a key. How this value is interpreted depends on
        /// the value of the uMapType parameter.</param>
        /// <param name="uMapType">The translation to be performed. The value of this parameter depends on the value
        /// of the uCode parameter.</param>
        /// <returns>The return value is either a scan code, a virtual-key code, or a character value, depending on the
        /// value of uCode and uMapType. If there is no translation, the return value is zero.</returns>
        [DllImport("user32.dll")]
        static extern uint MapVirtualKeyA(uint uCode, uint uMapType);

        /// <summary>
        /// uCode is a virtual-key code and is translated into an unshifted character value in the low-order word of the
        /// return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is
        /// no translation, the function returns 0.
        /// </summary>
        const uint MAPVK_VK_TO_CHAR = 2;
        /// <summary>
        /// uCode is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not
        /// distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no
        /// translation, the function returns 0.
        /// </summary>
        const uint MAPVK_VK_TO_VSC = 0;
        /// <summary>
        /// uCode is a scan code and is translated into a virtual-key code that does not distinguish between left- and
        /// right-hand keys. If there is no translation, the function returns 0.
        /// </summary>
        const uint MAPVK_VSC_TO_VK = 1;
        /// <summary>
        /// uCode is a scan code and is translated into a virtual-key code that distinguishes between left- and
        /// right-hand keys. If there is no translation, the function returns 0.
        /// </summary>
        const uint MAPVK_VSC_TO_VK_EX = 3;

        /// <summary>
        /// List of common scan codes associated with a QWERTY keyboard layout.
        /// </summary>
        public enum ScanCode
        {
            W = 0x11,
            A = 0x1E,
            S = 0x1F,
            D = 0x20
        }

        /// <summary>
        /// Gets the key where W would be on a QWERTY layout.
        /// </summary>
        public static Keys W { get { return Localize(ScanCode.W); } }
        /// <summary>
        /// Gets the key where A would be on a QWERTY layout.
        /// </summary>
        public static Keys A { get { return Localize(ScanCode.A); } }
        /// <summary>
        /// Gets the key where S would be on a QWERTY layout.
        /// </summary>
        public static Keys S { get { return Localize(ScanCode.S); } }
        /// <summary>
        /// Gets the key where D would be on a QWERTY layout.
        /// </summary>
        public static Keys D { get { return Localize(ScanCode.D); } }

        /// <summary>
        /// Based on the currently active keyboard layout, translates a scan code into the key at the position.
        /// </summary>
        /// <param name="scanCode">the keyboard scan code</param>
        /// <returns>the current Key associated with that scan code</returns>
        /// <remarks>If the user changes the keyboard layout, this value may change during runtime.</remarks>
        public static Keys Localize(ScanCode scanCode)
        {
            return (Keys)MapVirtualKeyA((uint)scanCode, MAPVK_VSC_TO_VK);
        }
    }
}
