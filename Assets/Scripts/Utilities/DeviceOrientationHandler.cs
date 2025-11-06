using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DeviceOrientationHandler : MonoBehaviour
    {
        public static CurrentDeviceFamily DeviceFamily { get; set; }

        [SerializeField] Camera _mainCamera;

        void OnEnable()
        {

            SetCurrentDevice();
        }



        void SetCurrentDevice()
        {
            var identifier = SystemInfo.deviceModel;
            if (identifier.StartsWith("iPhone") || identifier.StartsWith("iPad"))
            {

#if UNITY_IOS
          
            if (identifier.StartsWith("iPhone", StringComparison.Ordinal))
            {
                DeviceFamily = CurrentDeviceFamily.Phone;
                Debug.Log("Iphone -----------");
            }
            else if (identifier.StartsWith("iPad", StringComparison.Ordinal))
            {
                DeviceFamily = CurrentDeviceFamily.Tablet;
                Debug.Log("Ipad -----------");
            }

#endif
            }
            else
            {
                int n = GetDeviceId();

                if (n == 2)
                {
                    // Code for tablet
                    DeviceFamily = CurrentDeviceFamily.Tablet;
                }
                else if (n == 0)
                {
                    DeviceFamily = CurrentDeviceFamily.Other;

                }
                else if (n == 3)
                {
                    DeviceFamily = CurrentDeviceFamily.Phone_Flip_Fold;
                }
                else
                {
                    DeviceFamily = CurrentDeviceFamily.Phone;

                }

            }

        }


        public int GetDeviceId()
        {

            float val = 1.77f;

            float width = _mainCamera.GetComponent<Camera>().pixelWidth;

            float height = _mainCamera.GetComponent<Camera>().pixelHeight;


            if (width > height)
            {
                val = width / height;
            }
            else
            {
                val = height / width;
            }
            Debug.Log("VALUE" + val);

            if (val > 1.70f && val < 1.80f)
            {
                return 0;

            }
            else if (val > 2.0f && val < 2.2f)
            {
                return 1;
            }
            else if (val > 1.3f && val <= 1.6f)
            {
                return 2;
            }
            else if (val > 2.2)
            {
                return 3;
            }
            return 0;
        }


        private float DeviceDiagonalSizeInInches()
        {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            Debug.Log(Screen.dpi);

            float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

            return diagonalInches;
        }


    }
    // public enum CurrentDeviceOrientationEnu
    // {
    //     None,
    //     Portrait,
    //     PortraitUpsideDown,
    //     LandscapeLeft,
    //     LandscapeRight
    // }

    public enum CurrentDeviceFamily
    {
        None,
        Phone,
        Tablet,
        Phone_Flip_Fold,
        Other
    }
