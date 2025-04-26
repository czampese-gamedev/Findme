

using UnityEngine;
using UnityEditor;

namespace SimpleLowPolyCharachterAlice
{

    /// <summary>
    /// Unity editor class for the Alice clothes change
    /// </summary>
#if UNITY_EDITOR

    [CustomEditor(typeof(ClothChangeAlice))]
    public class ClothChangeEditorAlice : Editor
    {
        //Variables
        public int hSliderValueHair = 0;
        public int hSliderValueExpression = 0;
        public int hSliderValueShirt= 0;
        public int hSliderValuePants= 0;
        public int hSliderValueShoes= 0;

        public GUIStyle style;

        int oldValueHair=0;
        int oldValueExpression=0;
        int oldValueShirt=0;
        int oldValuePants=0;
        int oldValueShoes=0;

        private bool firstRun=true;

        //Editor screen sliders
        public override void OnInspectorGUI()
        {

            var SO = Selection.activeTransform;
            if (SO != null)
            {

            DrawDefaultInspector();

            ClothChangeAlice myScript = (ClothChangeAlice)target;

            GUI.skin.horizontalSlider = style;
            GUILayout.Label("Hair");        
            hSliderValueHair = EditorGUILayout.IntSlider(hSliderValueHair, 0, myScript.childCount(0)-1);

            GUI.skin.horizontalSlider = style;
            GUILayout.Label("Facial Expression");        
            hSliderValueExpression = EditorGUILayout.IntSlider(hSliderValueExpression, 0, myScript.childCount(1)-1);

            GUI.skin.horizontalSlider = style;
            GUILayout.Label("Shirt");        
            hSliderValueShirt = EditorGUILayout.IntSlider(hSliderValueShirt, 0, myScript.childCount(2)-1);
            
            GUI.skin.horizontalSlider = style;
            GUILayout.Label("Pants");        
            hSliderValuePants = EditorGUILayout.IntSlider(hSliderValuePants, 0, myScript.childCount(3)-1);

            GUI.skin.horizontalSlider = style;
            GUILayout.Label("Shoes");        
            hSliderValueShoes = EditorGUILayout.IntSlider(hSliderValueShoes, 0, myScript.childCount(4)-1);

            //Check first run
            if (firstRun){
            
            int value=myScript.clothCheck(0);
            hSliderValueHair=value;
            
            value=myScript.clothCheck(1);
            hSliderValueExpression=value;
            
            value=myScript.clothCheck(2);
            hSliderValueShirt=value;

            value=myScript.clothCheck(3);
            hSliderValuePants=value;

            value=myScript.clothCheck(4);
            hSliderValueShoes=value;

            firstRun=false;

            }

            if (hSliderValueHair !=oldValueHair) {
              oldValueHair=hSliderValueHair;
              myScript.setHair(hSliderValueHair);
            }

            if (hSliderValueExpression !=oldValueExpression) {
              oldValueExpression=hSliderValueExpression;
              myScript.setExpression(hSliderValueExpression);
            }

            if (hSliderValueShirt !=oldValueShirt) {
              oldValueShirt=hSliderValueShirt;
              myScript.setShirt(hSliderValueShirt);
            }

            if (hSliderValuePants !=oldValuePants) {
            oldValuePants=hSliderValuePants;
            myScript.setPants(hSliderValuePants);
            }

            if (hSliderValueShoes !=oldValueShoes) {
            oldValueShoes=hSliderValueShoes;
            myScript.setShoes(hSliderValueShoes);
            }

            }


        }


    }

    


#endif

}