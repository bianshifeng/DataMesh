using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMP.Unity.Mat
{
    public class MatAlphaBlink : MonoBehaviour
    {

        public float alphaMax = 1.0f;
        public float alphaMin = 0.3f;

        private bool m_isBlink = false;

        private Material m_mat;

        private float m_currentAlpha = 1.0f;
        private Color m_matColor;
        private float m_blinkSpeed = 0.03f;
        private float m_factor;

        private void Awake()
        {
            Renderer t_render = GetComponent<Renderer>();
            if(t_render != null)
            {
                m_mat = t_render.material;
            }
        }

        public void StartBlink()
        {
            if(m_mat != null)
            {
                m_currentAlpha = 1.0f;
                m_matColor = m_mat.color;
                m_factor = -m_blinkSpeed;
                m_isBlink = true;
            }
        }

        private void FixedUpdate()
        {
            if(m_isBlink)
            {
                if(m_mat != null)
                {
                    m_currentAlpha += m_factor;
                    if(m_currentAlpha > alphaMax)
                    {
                        m_currentAlpha = alphaMax;
                        m_factor = -m_blinkSpeed;
                    }

                    if(m_currentAlpha < alphaMin)
                    {
                        m_currentAlpha = alphaMin;
                        m_factor = m_blinkSpeed;
                    }

                    m_matColor.a = m_currentAlpha;
                    m_mat.color = m_matColor;

                }
            }
        }


    }

}

