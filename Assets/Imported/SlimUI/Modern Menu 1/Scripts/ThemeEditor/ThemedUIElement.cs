using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimUI.ModernMenu{
	[System.Serializable]
	public class ThemedUIElement : ThemedUI {
		[Header("Parameters")]
		Color outline;
		Image image;
		GameObject message;
		public enum OutlineStyle {solidThin, solidThick, dottedThin, dottedThick};
		public bool hasImage = false;
		public bool isText = false;

		protected override void OnSkinUI(){
			base.OnSkinUI();

			if(hasImage){
				image = GetComponent<Image>();
				image.color = themeController.currentColor;
			}

			message = gameObject;

			if(isText){
				// FIXED: Stop overriding color at runtime (it was making text invisible)
				// message.GetComponent<TextMeshPro>().color = themeController.textColor;
                // Try to handle UGUI too just in case 
                // var tmp = message.GetComponent<TextMeshProUGUI>();
                // if (tmp) tmp.color = Color.white;
			}
		}
	}
}