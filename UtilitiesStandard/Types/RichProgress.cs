using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public enum OperationStatus
    {
        WORKING = 0,
        ABORTED,
        FAILED
    }

    public interface IRichProgress : IProgress<double>, INotifyPropertyChanged
    {
        double Progress { get; set; }
        double MinProgress { get; set; }
        double MaxProgress { get; set; }
        double ProgressInPercents { get; }
        string Stage { get; set; }
        OperationStatus OperationStatus { get; set; }
        bool Optimize { get; set; }

        void AddProgress(double value, double weight = 1D);
        void Reset();
    }

    public class RichProgress : IRichProgress, INotifyPropertyChanged
    {
        double _lastProgress;
        double _globalWeidth = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Progress { get; set; }
        public double MinProgress { get; set; } = 0;
        public double MaxProgress { get; set; } = 1;
        public double ProgressInPercents { get; private set; }
        public string Stage { get; set; }
        public OperationStatus OperationStatus { get; set; }
        public bool Optimize { get; set; } = true;

        public RichProgress()
        {
            PropertyChanged += RichProgress_PropertyChanged;
        }

        void RichProgress_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.IsOneOf(nameof(Progress), nameof(MinProgress), nameof(MaxProgress)))
            {
                PropertyChanged?.Invoke(this, nameof(ProgressInPercents));
            }
        }

        public void Report(double value)
        {
            var oldP = getProgressInPercents(Progress, MinProgress, MaxProgress);
            var newP = getProgressInPercents(value, MinProgress, MaxProgress);
            _lastProgress = value;
            if (Optimize && newP - oldP < 0.001) // Because i dont want to rise the event too often
            {
                return;
            }
            else
            {
                Progress = value;
                ProgressInPercents = getProgressInPercents(Progress, MinProgress, MaxProgress);
            }
        }

        public void AddProgress(double value, double weight = 1D)
        {
            Report(_lastProgress + value * weight * _globalWeidth);
        }

        double getProgressInPercents(double progress, double minValue, double maxValue)
        {
            return (progress - minValue) / (maxValue - minValue);
        }

        public void Reset()
        {
            MinProgress = 0;
            MaxProgress = 1;
            _lastProgress = 0;
            Progress = 0;
            Stage = null;
            OperationStatus = default;
        }

        public IModeHolder<RichProgress> UseProgressWeightMode(double weight)
        {
            _globalWeidth *= weight;

            return new ModeHolder<RichProgress>(
                new DisposingAction(() => _globalWeidth /= weight), 
                this);
        }
    }
}
