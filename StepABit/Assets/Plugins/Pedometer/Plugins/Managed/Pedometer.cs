namespace PedometerU {

    using UnityEngine;
    using System;
    using Platforms;
    
    public sealed class Pedometer : IDisposable {

        
        public int updateCount { get; private set; }
        public static readonly IPedometer Implementation;

        private int initialSteps; // Some step counters count from device boot, so subtract the initial count we get
        private double initialDistance;
        private readonly StepCallback callback;

        public Pedometer (StepCallback callback) {
            if (Implementation == null) {
                Debug.LogError("Pedometer Error: Step counting is not supported on this platform");
                return;
            }
            if (callback == null) {
                Debug.LogError("Pedometer Error: Cannot create pedometer instance with null callback");
                return;
            }
            this.callback = callback;
            Implementation.OnStep += OnStep;
        }

        public void Dispose () {
            if (Implementation == null) {
                Debug.LogWarning("Pedometer Error: Step counting is not supported on this platform");
                return;
            }
            Implementation.OnStep -= OnStep;
        } 

        private void OnStep (int steps, double distance) {
            // Set initials and increment update count
            initialSteps = updateCount++ == 0 ? steps : initialSteps;
            initialDistance = steps == initialSteps ? distance : initialDistance;
            // If this is not the first step, then invoke the callback
            if (steps != initialSteps)
                callback(steps - initialSteps, distance - initialDistance);
        }

        static Pedometer () {
            // Create implementation for this platform
            Implementation =
            #if UNITY_IOS && !UNITY_EDITOR
            new PedometeriOS();
            #elif UNITY_ANDROID && !UNITY_EDITOR
            new PedometerAndroid();
            #else
            null;
            #endif
            PedometerUtility.Initialize();
        }
    }
}