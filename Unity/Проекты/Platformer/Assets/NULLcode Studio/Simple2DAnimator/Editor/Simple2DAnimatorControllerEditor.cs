/********************************************************/
/**  © 2020 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using UnityEngine;
using UnityEditor;

public class Simple2DAnimatorControllerEditor : EditorWindow
{
    Vector2 scrollPos;
    float step = .02f;
    float previewScale = .5f;
    double timeout;
    Simple2DAnimatorController controller;
    Texture2D texture;
    string[] animList;
    string animName;
    int animIX, index, tmp_index, animIXLast;
    bool preview;

    public static void OpenWindow(Simple2DAnimatorController animatorController)
    {
        Simple2DAnimatorControllerEditor editor = GetWindow<Simple2DAnimatorControllerEditor>();
        GUIContent titleContent = new GUIContent("• 2D Animator Controller Editor");
        editor.minSize = new Vector2(800, 600);
        editor.titleContent = titleContent;
        editor.Init(animatorController);
    }

    void Init(Simple2DAnimatorController animatorController)
    {
        controller = animatorController;

        if (controller.animatorData.animatoins != null && controller.animatorData.animatoins.Length > 0)
        {
            animList = new string[controller.animatorData.animatoins.Length];
            for (int i = 0; i < controller.animatorData.animatoins.Length; i++)
            {
                animList[i] = controller.animatorData.animatoins[i].nameID;
                if (controller.animatorData.animatoins[i].step <= 0)
                {
                    controller.animatorData.animatoins[i].step = step;
                }
            }
        }
        else
        {
            controller.animatorData.animatoins = new Simple2DAnimatorDataList[] { };
            animList = new string[] { };
        }

        animIX = 0;
        index = 0;
        animIXLast = -1;
    }

    void AddAnimation()
    {
        if (string.IsNullOrEmpty(animName)) return;
        Simple2DAnimatorDataList part = new Simple2DAnimatorDataList();
        part.nameID = animName;
        part.step = step;
        part.loop = true;
        part.active = new bool[] { };
        part.sendInt = new int[] { };
        part.sendFloat = new float[] { };
        part.sendString = new string[] { };
        part.sprites = new Sprite[] { };
        ArrayUtility.Add<Simple2DAnimatorDataList>(ref controller.animatorData.animatoins, part);
        ArrayUtility.Add<string>(ref animList, animName);
        animName = string.Empty;
        index = 0;
        tmp_index = 0;
        animIX = animList.Length - 1;
    }

    GUIStyle DelStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.white;
        style.fixedWidth = 100;
        return style;
    }

    GUIStyle PopupStyle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.popup);
        style.normal.textColor = Color.black;
        style.fixedHeight = 20;
        return style;
    }

    void DelAnimation()
    {
        ArrayUtility.RemoveAt<Simple2DAnimatorDataList>(ref controller.animatorData.animatoins, animIX);
        ArrayUtility.RemoveAt<string>(ref animList, animIX);
        index = 0;
        tmp_index = 0;
        animIX = animList.Length - 1;
    }

    void OnGUI()
    {
        
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Добавить анимацию:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        animName = EditorGUILayout.TextField("Название: ", animName);
        if (GUILayout.Button("Добавить..."))
        {
            preview = false;
            AddAnimation();
            Repaint();
        }
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Сохранить проект"))
        {
            preview = false;
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        if (animList.Length > 0)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Редактирование анимации:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Текущая анимация:");
            animIX = EditorGUILayout.Popup(animIX, animList, PopupStyle());

            if (animIXLast != animIX)
            {
                index = 0;
                tmp_index = 0;
            }

            animIXLast = animIX;

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Удалить", DelStyle()))
            {
                preview = false;
                DelAnimation();
                return;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("\nПеретащите сюда спрайты, для создания анимации.\n\nВнимание! Необходимо перетаскивать отдельные кадры, не атласы!\n", MessageType.Info);
            Rect dropRect = GUILayoutUtility.GetLastRect();
            if ((Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated) && dropRect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (Event.current.type == EventType.DragPerform)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        if (DragAndDrop.objectReferences[i] is Texture2D || DragAndDrop.objectReferences[i] is Sprite)
                        {
                            ArrayUtility.Add<Sprite>(ref controller.animatorData.animatoins[animIX].sprites, AssetDatabase.LoadAssetAtPath<Sprite>(DragAndDrop.paths[i]));
                            ArrayUtility.Add<string>(ref controller.animatorData.animatoins[animIX].sendString, string.Empty);
                            ArrayUtility.Add<int>(ref controller.animatorData.animatoins[animIX].sendInt, 0);
                            ArrayUtility.Add<bool>(ref controller.animatorData.animatoins[animIX].active, false);
                            ArrayUtility.Add<float>(ref controller.animatorData.animatoins[animIX].sendFloat, 0);
                        }
                    }
                }

                Event.current.Use();
            }
            
            if (controller.animatorData.animatoins[animIX].sprites.Length > 0)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                controller.animatorData.animatoins[animIX].step = EditorGUILayout.Slider("Шаг кадра: ", controller.animatorData.animatoins[animIX].step, .01f, 1);
                controller.animatorData.animatoins[animIX].loop = EditorGUILayout.ToggleLeft(" Зацикленная анимация?", controller.animatorData.animatoins[animIX].loop);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                if (!preview)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Управление кадром [" + index + "]:");
                    if (GUILayout.Button("Событие"))
                    {
                        controller.animatorData.animatoins[animIX].active[index] = !controller.animatorData.animatoins[animIX].active[index];
                    }
                    if (GUILayout.Button("Заменить"))
                    {
                        EditorGUIUtility.ShowObjectPicker<Sprite>(controller.animatorData.animatoins[animIX].sprites[index], true, "", EditorGUIUtility.GetControlID(FocusType.Passive) + 100);
                    }
                    if (GUILayout.Button("Удалить"))
                    {
                        ArrayUtility.RemoveAt<Sprite>(ref controller.animatorData.animatoins[animIX].sprites, index);
                        ArrayUtility.RemoveAt<string>(ref controller.animatorData.animatoins[animIX].sendString, index);
                        ArrayUtility.RemoveAt<int>(ref controller.animatorData.animatoins[animIX].sendInt, index);
                        ArrayUtility.RemoveAt<bool>(ref controller.animatorData.animatoins[animIX].active, index);
                        ArrayUtility.RemoveAt<float>(ref controller.animatorData.animatoins[animIX].sendFloat, index);
                        if (index > controller.animatorData.animatoins[animIX].sprites.Length - 1) index = controller.animatorData.animatoins[animIX].sprites.Length - 1;
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();

                    if (controller.animatorData.animatoins[animIX].active[index])
                    {
                        EditorGUILayout.BeginHorizontal();
                        controller.animatorData.animatoins[animIX].sendInt[index] = EditorGUILayout.IntField("Int: ", controller.animatorData.animatoins[animIX].sendInt[index]);
                        controller.animatorData.animatoins[animIX].sendFloat[index] = EditorGUILayout.FloatField("Float: ", controller.animatorData.animatoins[animIX].sendFloat[index]);
                        controller.animatorData.animatoins[animIX].sendString[index] = EditorGUILayout.TextField("String: ", controller.animatorData.animatoins[animIX].sendString[index]);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                    }
                }
                
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < controller.animatorData.animatoins[animIX].sprites.Length; i++)
                {
                    if (GUILayout.Button(controller.animatorData.animatoins[animIX].sprites[i].texture, GUILayout.Height(controller.animatorData.animatoins[animIX].sprites[i].texture.height * previewScale), GUILayout.Width(controller.animatorData.animatoins[animIX].sprites[i].texture.width * previewScale)))
                    {
                        preview = false;
                        index = i;
                        Repaint();
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                if (preview && EditorApplication.timeSinceStartup > timeout)
                {
                    index++;
                    timeout = EditorApplication.timeSinceStartup + controller.animatorData.animatoins[animIX].step;
                    if (index > controller.animatorData.animatoins[animIX].sprites.Length - 1)
                    {
                        index = 0;
                    }
                }

                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(preview ? "Остановить" : "Просмотр", GUILayout.Height(50), GUILayout.Width(100)))
                {
                    preview = !preview;

                    if (preview)
                    {
                        tmp_index = index;
                    }
                    else
                    {
                        index = tmp_index;
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                previewScale = EditorGUILayout.Slider("Размер превью: ", previewScale, .1f, 1);
                EditorGUILayout.LabelField(controller.animatorData.animatoins[animIX].sprites[index].name, SpriteNameStyle());
                EditorGUI.DrawTextureTransparent(new Rect(EditorGUIUtility.currentViewWidth / 2 - (controller.animatorData.animatoins[animIX].sprites[index].texture.width * previewScale / 2), GUILayoutUtility.GetLastRect().yMax + 5, controller.animatorData.animatoins[animIX].sprites[index].texture.width * previewScale, controller.animatorData.animatoins[animIX].sprites[index].texture.height * previewScale), controller.animatorData.animatoins[animIX].sprites[index].texture);
            }
        }
        
        if (Event.current.commandName == "ObjectSelectorUpdated")
        {
            controller.animatorData.animatoins[animIX].sprites[index] = (Sprite)EditorGUIUtility.GetObjectPickerObject();
            Repaint();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Copyright © 2020 NULLcode Studio", EditorStyles.centeredGreyMiniLabel);
    }

    GUIStyle SpriteNameStyle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        return style;
    }

    void Update()
    {
        if (preview)
        {
            Repaint();
        }
    }
}
