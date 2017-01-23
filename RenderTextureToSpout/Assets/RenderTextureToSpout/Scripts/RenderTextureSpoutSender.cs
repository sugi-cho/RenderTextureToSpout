using UnityEngine;
using System.Collections;
using System;

namespace Spout
{
    public class RenderTextureSpoutSender : MonoBehaviour
    {
        public enum TextureFormat { DXGI_FORMAT_R32G32B32A32_FLOAT = 2, DXGI_FORMAT_R10G10B10A2_UNORM = 24, DXGI_FORMAT_R8G8B8A8_UNORM = 28, DXGI_FORMAT_B8G8R8A8_UNORM = 87 }
        public string sharingName = "UnitySender";
        public TextureFormat textureFormat = TextureFormat.DXGI_FORMAT_R8G8B8A8_UNORM;
        bool senderIsCreated;

        public Texture texture;

        private int _createAttempts = 5;
        private int _attempts = 0;

        bool textureSizeHasChanged
        {
            get
            {
                var b = !(texture.width == width && texture.height == height);
                width = texture.width;
                height = texture.height;
                return b;
            }
        }
        int width = 0;
        int height = 0;

        void OnDisable()
        {
            _CloseSender();
        }

        public void SetTexture(Texture tex)
        {
            texture = tex;
        }

        void Update()
        {
            if (texture == null) return;
            if (textureSizeHasChanged) _CloseSender();
            if (!senderIsCreated) _CreateSender(texture);
            if (senderIsCreated) Spout.instance.UpdateSender(sharingName, texture);
        }

        private void _CreateSender(Texture texture)
        {
            if (texture == null) return;
            if (!Spout.instance.enabled) return;

            if (!senderIsCreated)
            {
                Debug.Log("Sender is not created, creating one");
                senderIsCreated = Spout.instance.CreateSender(sharingName, texture, (int)textureFormat);
            }

            _attempts++;
            if (_attempts > _createAttempts) Debug.LogWarning(String.Format("There are problems with creating the sender {0}. Please check your settings or restart Unity.", sharingName));

            Spout.instance.OnSenderStopped -= OnSenderStoppedDelegate;
            Spout.instance.OnSenderStopped += OnSenderStoppedDelegate;

            Spout.instance.OnAllSendersStopped -= OnAllSendersStoppedDelegate;
            Spout.instance.OnAllSendersStopped += OnAllSendersStoppedDelegate;

            Spout.instance.OnEnabled -= _OnSpoutEnabled;
            Spout.instance.OnEnabled += _OnSpoutEnabled;
        }

        private void _OnSpoutEnabled()
        {
            //Debug.Log("SpoutSender._OnSpoutEnabled");
            if (enabled)
            {
                //force a reconnection
                enabled = !enabled;
                enabled = !enabled;
            }
        }

        private void _CloseSender()
        {
            Debug.Log("SpoutSender._CloseSender:" + sharingName);
            if (senderIsCreated) Spout.CloseSender(sharingName);
            _CloseSenderCleanUpData();
        }

        private void OnSenderStoppedDelegate(object sender, TextureShareEventArgs e)
        {
            //Debug.Log("SpoutSender.OnSenderStoppedDelegate:"+e.sharingName);
            if (e.sharingName == sharingName)
            {
                _CloseSenderCleanUpData();
            }
        }

        private void OnAllSendersStoppedDelegate()
        {
            _CloseSenderCleanUpData();
        }

        private void _CloseSenderCleanUpData()
        {
            senderIsCreated = false;
        }
    }
}