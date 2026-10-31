using System;
using UnityEngine;

namespace SonBirUmutDemo.UI
{
    // Small IMGUI skin; textures belong to this HUD and are released when the scene closes.
    public sealed class DemoGuiTheme : IDisposable
    {
        public static readonly Color Surface = new Color(.075f, .12f, .13f, .98f);
        public static readonly Color Card = new Color(.12f, .18f, .19f, 1f);
        public static readonly Color Text = new Color(.95f, .94f, .86f);
        public static readonly Color Muted = new Color(.69f, .76f, .73f);
        public static readonly Color Green = new Color(.47f, .78f, .57f);
        public static readonly Color Gold = new Color(.91f, .73f, .39f);
        public static readonly Color Blue = new Color(.43f, .74f, .87f);
        public static readonly Color Red = new Color(.92f, .50f, .37f);
        public readonly GUIStyle Title, Heading, ChoiceTitle, Body, Small, Tiny, Button, QuietButton;
        private readonly Texture2D normal, hover, pressed, disabled, quiet;
        private readonly GUIStyle unavailable;
        public DemoGuiTheme()
        {
            Title = Label(25, Text, true);
            Heading = Label(20, Text, true);
            ChoiceTitle = Label(17, Text, true);
            Body = Label(17, Text);
            Small = Label(15, Muted);
            Tiny = Label(14, Text);
            normal = Solid(new Color(.28f, .49f, .37f));
            hover = Solid(new Color(.35f, .61f, .45f));
            pressed = Solid(new Color(.20f, .37f, .28f));
            disabled = Solid(new Color(.20f, .25f, .25f));
            quiet = Solid(new Color(.18f, .26f, .27f));
            Button = new GUIStyle(GUI.skin.button)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                border = new RectOffset(),
                padding = new RectOffset(10, 10, 6, 6)
            };
            Button.normal.background = normal;
            Button.normal.textColor = Text;
            Button.hover.background = hover;
            Button.hover.textColor = Text;
            Button.active.background = pressed;
            Button.active.textColor = Text;
            Button.focused.background = hover;
            Button.focused.textColor = Text;
            Button.onNormal.background = normal;
            Button.onNormal.textColor = Text;
            QuietButton = new GUIStyle(Button);
            QuietButton.normal.background = quiet;
            unavailable = new GUIStyle(Button);
            unavailable.normal.background = disabled;
            unavailable.hover.background = disabled;
            unavailable.active.background = disabled;
            unavailable.normal.textColor = Muted;
        }

        public void Panel(Rect area, Color? accent = null)
        {
            Fill(new Rect(area.x + 3, area.y + 4, area.width, area.height), new Color(0, 0, 0, .28f));
            Fill(area, Surface);
            if (accent.HasValue)
                Fill(new Rect(area.x, area.y, area.width, 3), accent.Value);
        }

        public static void Fill(Rect area, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(area, Texture2D.whiteTexture);
            GUI.color = previous;
        }

        public static void Bar(Rect area, float ratio, Color color)
        {
            Fill(area, new Color(.22f, .29f, .29f));
            Fill(new Rect(area.x, area.y, area.width * Mathf.Clamp01(ratio), area.height), color);
        }

        public bool Action(Rect area, string title, bool enabled = true, bool quietStyle = false)
        {
            bool previous = GUI.enabled;
            GUI.enabled = previous && enabled;
            bool click = GUI.Button(area, title, !enabled ? unavailable : quietStyle ? QuietButton : Button);
            GUI.enabled = previous;
            return click;
        }

        public void EffectChip(Rect area, string name, int delta, bool emissions = false)
        {
            bool beneficial = emissions ? delta < 0 : delta > 0;
            Color color = delta == 0 ? Muted : beneficial ? Green : Red;
            Fill(area, Card);
            Color previous = Tiny.normal.textColor;
            Tiny.normal.textColor = color;
            GUI.Label(area, name + " " + delta.ToString("+0;-0;0"), Tiny);
            Tiny.normal.textColor = previous;
        }

        private static GUIStyle Label(int size, Color color, bool bold = false)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                wordWrap = true,
                fontStyle = bold ? FontStyle.Bold : FontStyle.Normal,
                padding = new RectOffset(0, 0, 0, 0)
            };
            style.normal.textColor = color;
            return style;
        }

        private static Texture2D Solid(Color color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public void Dispose()
        {
            foreach (var texture in new[]
            {
                normal,
                hover,
                pressed,
                disabled,
                quiet
            }

            )
                if (texture != null)
                {
                    if (Application.isPlaying)
                        UnityEngine.Object.Destroy(texture);
                    else
                        UnityEngine.Object.DestroyImmediate(texture);
                }
        }
    }
}
