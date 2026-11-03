using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sunfall
{
    public sealed class Bindings
    {
        public readonly string[] Names={"왼쪽 이동","오른쪽 이동","공격","점프","구르기","스킬 슬롯 1","스킬 슬롯 2","스킬 슬롯 3","스킬 슬롯 4"};
        public Key[] Keys {get;private set;}
        static Key[] Defaults(){return new[]{Key.LeftArrow,Key.RightArrow,Key.LeftCtrl,Key.LeftAlt,Key.LeftShift,Key.F,Key.Q,Key.E,Key.R};}
        public Bindings()
        {
            Keys=Defaults();var candidate=Defaults();
            for(int i=0;i<Keys.Length;i++)
            {
                Key loaded=Normalize((Key)PlayerPrefs.GetInt("sunfall.key."+i,(int)Keys[i]));
                if(!Available(candidate,i,loaded))
                {
                    loaded=Keys[i];
                    if(!Available(candidate,i,loaded))
                        foreach(var spare in new[]{Key.T,Key.Y,Key.G,Key.H,Key.J,Key.K,Key.L,Key.Z,Key.X,Key.C,Key.V,Key.B})
                            if(Available(candidate,i,spare)){loaded=spare;break;}
                }
                candidate[i]=loaded;
            }
            Keys=candidate;
        }
        static bool Available(Key[] assigned,int count,Key key)
        {
            if(!Valid(key))return false;
            for(int i=0;i<count;i++)if(assigned[i]==key)return false;
            return true;
        }
        public static Key Normalize(Key k){return k==Key.RightCtrl?Key.LeftCtrl:k==Key.RightAlt?Key.LeftAlt:k==Key.RightShift?Key.LeftShift:k;}
        static bool Valid(Key k){return k!=Key.None&&k!=Key.Escape&&k!=Key.I&&k!=Key.K&&k!=Key.UpArrow&&k!=Key.DownArrow&&k!=Key.Space&&!(k>=Key.F1&&k<=Key.F8)&&k!=Key.F10&&Enum.IsDefined(typeof(Key),k);}
        public bool Set(int action,Key key)
        {
            key=Normalize(key);
            if(action<0||action>=Keys.Length||!Valid(key))return false;
            for(int i=0;i<Keys.Length;i++)if(i!=action&&Keys[i]==key)return false;
            Keys[action]=key;Save();return true;
        }
        public void Reset(){Keys=Defaults();Save();}
        void Save(){for(int i=0;i<Keys.Length;i++)PlayerPrefs.SetInt("sunfall.key."+i,(int)Keys[i]);PlayerPrefs.Save();}
        public bool Held(int action)
        {
            var k=Keyboard.current;if(k==null)return false;
            Key key=Keys[action];
            return k[key].isPressed||(key==Key.LeftCtrl&&k.rightCtrlKey.isPressed)||(key==Key.LeftAlt&&k.rightAltKey.isPressed)||(key==Key.LeftShift&&k.rightShiftKey.isPressed);
        }
        public bool Pressed(int action)
        {
            var k=Keyboard.current;if(k==null)return false;Key key=Keys[action];
            return k[key].wasPressedThisFrame||(key==Key.LeftCtrl&&k.rightCtrlKey.wasPressedThisFrame)||(key==Key.LeftAlt&&k.rightAltKey.wasPressedThisFrame)||(key==Key.LeftShift&&k.rightShiftKey.wasPressedThisFrame);
        }
        public string Label(int action)
        {
            switch(Keys[action]){case Key.LeftCtrl:return "Ctrl";case Key.LeftAlt:return "Alt";case Key.LeftShift:return "Shift";case Key.LeftArrow:return "←";case Key.RightArrow:return "→";default:return Keys[action].ToString();}
        }
    }
}
