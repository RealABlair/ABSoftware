using System;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

namespace ABSoftware.UI.Animations
{
    public class UIAnimator
    {
        /*void SetFieldValue(Control control, string fieldName, object value)
        {
            Type controlType = control.GetType();
            PropertyInfo property = controlType.GetProperty(fieldName);
            property.SetValue(control, value);
        }*/

        public ArrayList<Animation> runningAnimations = new ArrayList<Animation>();

        public void StartAnimation(Animation animation)
        {
            runningAnimations.Add(animation);
            Thread t = new Thread(() =>
            {
                bool stop = false;

                while(!stop && !animation.forceStop)
                {
                    animation.OnUpdate((float)(DateTime.Now - animation.animationStartTime).TotalSeconds, out animation.progress, out stop);
                    Thread.Sleep(1);
                }
                runningAnimations.Remove(animation);
            });
            t.IsBackground = true;
            t.Start();
        }

        public void KillAnimation(string animationId)
        {
            Animation[] anim = runningAnimations.GetElements();
            for(int i = 0; i < anim.Length; i++)
            {
                if (anim[i].animationId.Equals(animationId) && anim[i] != null)
                {
                    anim[i].forceStop = true;
                }
            }
        }
    }

    public struct AnimationData
    {
        public Control control;
        public string fieldName;
        public object targetValue;
        
        public AnimationData(Control control, string fieldName, object targetValue)
        {
            this.control = control;
            this.fieldName = fieldName;
            this.targetValue = targetValue;
        }
    }

    public class Animation
    {
        public string animationId;
        public bool forceStop = false;

        public AnimationData[] data { get; private set; }
        public DateTime animationStartTime { get; private set; }
        public float animationTime;
        public float progress;

        public Animation(string animationId, float animationTime, params AnimationData[] data)
        {
            this.animationId = animationId;
            this.animationStartTime = DateTime.Now;
            this.animationTime = animationTime;
            this.data = data;
        }

        public virtual void OnUpdate(float timeElapsed, out float progress, out bool isDone) { progress = 1f; isDone = true; }
    }

    public class TransitionAnimation : Animation
    {
        public object[] startValue;
        private TransitionType type;
        private float progressOffset = 0f;

        public TransitionAnimation(string animationId, float animationTime, TransitionType type, params AnimationData[] data) : base(animationId, animationTime, data)
        {
            startValue = new object[data.Length];
            for(int i = 0; i < data.Length; i++)
            {
                Type controlType = data[i].control.GetType();
                PropertyInfo property = controlType.GetProperty(data[i].fieldName);
                startValue[i] = property.GetValue(data[i].control);
            }
        }

        public TransitionAnimation(string animationId, float animationTime, float progress, object[] startValues, TransitionType type, params AnimationData[] data) : base(animationId, animationTime, data)
        {
            this.type = type;
            this.progressOffset = progress;
            startValue = new object[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                startValue[i] = startValues[i];
            }
        }

        public void UpdateProgress(float newProgress)
        {
            this.progressOffset = newProgress;
        }

        public override void OnUpdate(float timeElapsed, out float progress, out bool isDone)
        {
            float localProgress = progressOffset + (timeElapsed / animationTime);
            progress = localProgress;
            if(localProgress >= 1f)
            {
                localProgress = 1f;
                progress = 1f;
                isDone = true;
            }
            else
            {
                isDone = false;
                for(int i = 0; i < data.Length; i++)
                {
                    Type controlType = data[i].control.GetType();
                    PropertyInfo property = controlType.GetProperty(data[i].fieldName);
                    object value = GetValue(property.PropertyType, localProgress, i);
                    data[i].control.Invoke(new Action(() => { property.SetValue(data[i].control, value); }));
                }
            }
        }

        private object GetValue(Type fieldType, float progress, int dataIndex)
        {
            object ret = null;
            if(fieldType == typeof(int))
            {
                ret = (int)CalculateTransition((float)(int)startValue[dataIndex], (float)(int)data[dataIndex].targetValue, progress);
            }

            return ret;
        }

        private float CalculateTransition(float value, float target, float progress)
        {
            switch(type)
            {
                case TransitionType.Linear: return Linear(value, target, progress);
                case TransitionType.EaseInOut: return EaseInOut(value, target, progress);
                case TransitionType.EaseIn: return EaseIn(value, target, progress);
                case TransitionType.EaseOut: return EaseOut(value, target, progress);
                default: return Linear(value, target, progress);
            }
        }

        private float Linear(float value, float target, float progress)
        {
            return value + (target - value) * progress;
        }

        private float EaseIn(float value, float target, float progress)
        {
            if(progress <= 0.5f)
                return Linear(value, target, 2 * progress * progress);
            return Linear(value, target, progress);
        }

        private float EaseOut(float value, float target, float progress)
        {
            if (progress > 0.5f)
            {
                float p = progress - 0.5f;
                return Linear(value, target, 2f * p * (1f - p) + 0.5f);
            }
            return Linear(value, target, progress);
        }

        private float EaseInOut(float value, float target, float progress)
        {
            if (progress <= 0.5f)
                return Linear(value, target, 2 * progress * progress);
            else if (progress > 0.5f)
            {
                float p = progress - 0.5f;
                return Linear(value, target, 2f * p * (1f - p) + 0.5f);
            }
            return Linear(value, target, progress);
        }

        public enum TransitionType
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut
        }
    }
}
