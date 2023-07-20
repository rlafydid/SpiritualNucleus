using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

    public class PointerButton : Button
    {
		private System.Action<GameObject, object> downCallBack;
		private System.Action<GameObject, object> upCallBack;

		private System.Action noParamDownCallBack;
		private System.Action noParamUpCallBack;

		private object param;

		public void AddPointerClick(System.Action<GameObject, object> downCallBack, System.Action<GameObject, object> upCallBack = null, object param = null)
        {
			this.downCallBack = downCallBack;
			this.upCallBack = upCallBack;
			this.param = param;
        }

		public void AddPointerClick(System.Action downCallBack, System.Action upCallBack = null)
		{
			this.noParamDownCallBack = downCallBack;
			this.noParamUpCallBack = upCallBack;
		}

        public void AddClick()
        {

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
				if (downCallBack != null) {
						downCallBack (gameObject, param);
				} else if (noParamDownCallBack != null) {
						noParamDownCallBack ();
				}
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
				if (upCallBack != null) {
						upCallBack (gameObject, param);
				} else if (noParamUpCallBack != null) {
						noParamUpCallBack ();
				}
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
				base.OnPointerClick (eventData);
        }
    }
