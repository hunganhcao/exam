using UnityEngine;
#if MM_CINEMACHINE
using Cinemachine;
#endif
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This class will allow you to trigger zooms on your cinemachine camera by sending MMCameraZoomEvents from any other class
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachineZoom2D")]
#if MM_CINEMACHINE
    [RequireComponent(typeof(Cinemachine.CinemachineVirtualCamera))]
#endif
    public class MMCinemachineZoom2D : MonoBehaviour
    {
        [Header("Channel")]
        [MMFInspectorGroup("Shaker Settings", true, 3)]
        /// whether to listen on a channel defined by an int or by a MMChannel scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what.
        /// MMChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable
        [Tooltip("whether to listen on a channel defined by an int or by a MMChannel scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what. " +
                 "MMChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable")]
        public MMChannelModes ChannelMode = MMChannelModes.Int;
        /// the channel to listen to - has to match the one on the feedback
        [Tooltip("the channel to listen to - has to match the one on the feedback")]
        [MMFEnumCondition("ChannelMode", (int)MMChannelModes.Int)]
        public int Channel = 0;
        /// the MMChannel definition asset to use to listen for events. The feedbacks targeting this shaker will have to reference that same MMChannel definition to receive events - to create a MMChannel,
        /// right click anywhere in your project (usually in a Data folder) and go MoreMountains > MMChannel, then name it with some unique name
        [Tooltip("the MMChannel definition asset to use to listen for events. The feedbacks targeting this shaker will have to reference that same MMChannel definition to receive events - to create a MMChannel, " +
                 "right click anywhere in your project (usually in a Data folder) and go MoreMountains > MMChannel, then name it with some unique name")]
        [MMFEnumCondition("ChannelMode", (int)MMChannelModes.MMChannel)]
        public MMChannel MMChannelDefinition = null;

        [Header("Transition Speed")]
        /// the animation curve to apply to the zoom transition
        [Tooltip("the animation curve to apply to the zoom transition")]
        public AnimationCurve ZoomCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        [Header("Test Zoom")]
        /// the mode to apply the zoom in when using the test button in the inspector
        [Tooltip("the mode to apply the zoom in when using the test button in the inspector")]
        public MMCameraZoomModes TestMode;
        /// the target OrthographicSize to apply the zoom in when using the test button in the inspector
        [Tooltip("the target OrthographicSize to apply the zoom in when using the test button in the inspector")]
        public float TestOrthographicSize = 30f;
        /// the transition duration to apply the zoom in when using the test button in the inspector
        [Tooltip("the transition duration to apply the zoom in when using the test button in the inspector")]
        public float TestTransitionDuration = 0.1f;
        /// the duration to apply the zoom in when using the test button in the inspector
        [Tooltip("the duration to apply the zoom in when using the test button in the inspector")]
        public float TestDuration = 0.05f;

        [MMFInspectorButton("TestZoom")]
        /// an inspector button to test the zoom in play mode
        public bool TestZoomButton;

        public virtual float GetTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
        public virtual float GetDeltaTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

        public TimescaleModes TimescaleMode { get; set; }

#if MM_CINEMACHINE
        protected Cinemachine.CinemachineVirtualCamera _virtualCamera;
        protected float _initialOrthographicSize;
        protected MMCameraZoomModes _mode;
        protected bool _zooming = false;
        protected float _startOrthographicSize;
        protected float _transitionDuration;
        protected float _duration;
        protected float _targetOrthographicSize;
        protected float _delta = 0f;
        protected int _direction = 1;
        protected float _reachedDestinationTimestamp;
        protected bool _destinationReached = false;

        /// <summary>
        /// On Awake we grab our virtual camera
        /// </summary>
        protected virtual void Awake()
        {
            _virtualCamera = this.gameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            _initialOrthographicSize = _virtualCamera.m_Lens.OrthographicSize;
        }

        /// <summary>
        /// On Update if we're zooming we modify our field of view accordingly
        /// </summary>
        protected virtual void Update()
        {
            if (!_zooming)
            {
                return;
            }

            if (_virtualCamera.m_Lens.OrthographicSize != _targetOrthographicSize)
            {
                _delta += GetDeltaTime() / _transitionDuration;
                _virtualCamera.m_Lens.OrthographicSize = Mathf.LerpUnclamped(_startOrthographicSize, _targetOrthographicSize, ZoomCurve.Evaluate(_delta));
            }
            else
            {
                if (!_destinationReached)
                {
                    _reachedDestinationTimestamp = GetTime();
                    _destinationReached = true;
                }

                if ((_mode == MMCameraZoomModes.For) && (_direction == 1))
                {
                    if (GetTime() - _reachedDestinationTimestamp > _duration)
                    {
                        _direction = -1;
                        _startOrthographicSize = _targetOrthographicSize;
                        _targetOrthographicSize = _initialOrthographicSize;
                        _delta = 0f;
                    }
                }
                else
                {
                    _zooming = false;
                }
            }
        }

        /// <summary>
        /// A method that triggers the zoom, ideally only to be called via an event, but public for convenience
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="newOrthographicSize"></param>
        /// <param name="transitionDuration"></param>
        /// <param name="duration"></param>
        public virtual void Zoom(MMCameraZoomModes mode, float newOrthographicSize, float transitionDuration, float duration, bool useUnscaledTime, bool relative = false)
        {
            if (_zooming)
            {
                return;
            }

            _zooming = true;
            _delta = 0f;
            _mode = mode;

            TimescaleMode = useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled;
            _startOrthographicSize = _virtualCamera.m_Lens.OrthographicSize;
            _transitionDuration = transitionDuration;
            _duration = duration;
            _transitionDuration = transitionDuration;
            _direction = 1;
            _destinationReached = false;

            switch (mode)
            {
                case MMCameraZoomModes.For:
                    _targetOrthographicSize = newOrthographicSize;
                    break;

                case MMCameraZoomModes.Set:
                    _targetOrthographicSize = newOrthographicSize;
                    break;

                case MMCameraZoomModes.Reset:
                    _targetOrthographicSize = _initialOrthographicSize;
                    break;
            }

            if (relative)
            {
                _targetOrthographicSize += _initialOrthographicSize;
            }
        }

        /// <summary>
        /// The method used by the test button to trigger a test zoom
        /// </summary>
        protected virtual void TestZoom()
        {
            Zoom(TestMode, TestOrthographicSize, TestTransitionDuration, TestDuration, false);
        }

        /// <summary>
        /// When we get an MMCameraZoomEvent we call our zoom method 
        /// </summary>
        /// <param name="zoomEvent"></param>
        public virtual void OnCameraZoomEvent(MMCameraZoomModes mode, float newOrthographicSize, float transitionDuration, float duration, MMChannelData channelData, bool useUnscaledTime, bool stop = false, bool relative = false, bool restore = false)
        {
            if (!MMChannel.Match(channelData, ChannelMode, Channel, MMChannelDefinition))
            {
                return;
            }
            if (stop)
            {
                _zooming = false;
                return;
            }
            if (restore)
            {
                _virtualCamera.m_Lens.OrthographicSize = _initialOrthographicSize;
                return;
            }
            this.Zoom(mode, newOrthographicSize, transitionDuration, duration, useUnscaledTime, relative);
        }

        /// <summary>
        /// Starts listening for MMCameraZoomEvents
        /// </summary>
        protected virtual void OnEnable()
        {
            MMCameraZoomEvent.Register(OnCameraZoomEvent);
        }

        /// <summary>
        /// Stops listening for MMCameraZoomEvents
        /// </summary>
        protected virtual void OnDisable()
        {
            MMCameraZoomEvent.Unregister(OnCameraZoomEvent);
        }
#endif
    }
}