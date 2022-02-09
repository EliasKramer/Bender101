using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Delay
{
    private DateTime _actionStartedDate;
    private DateTime _lastTimeActionFinishedDate;
    private bool _isDoingAction;
    private float _delay;
    /// <summary>
    /// Can be used for setting time constraints for certain actions
    /// </summary>
    /// <param name="delay">Delay in Milliseconds (shall not be less or equal zero)</param>
    /// <param name="mustWaitDelayBeforeAction">Should it wait the delay after creating the object, or shall it be available immediatly?</param>
    /// <exception cref="ArgumentOutOfRangeException">throws exception when delay is less or equal zero</exception>
    public Delay(float delay, bool mustWaitDelayBeforeAction)
    {
        if(delay <= 0)
        {
            throw new ArgumentOutOfRangeException("A delay cant be negative");
        }
        _isDoingAction = false;
        _delay = delay;

        if(mustWaitDelayBeforeAction)
        {
            _lastTimeActionFinishedDate = DateTime.UtcNow;
        }
        else
        {
            _lastTimeActionFinishedDate = DateTime.MinValue;
        }
    }
    /// <summary>
    /// If there is enough delay between the last stop of action,
    /// it will start the action - the timer
    /// </summary>
    /// <returns>returns if the action could be started</returns>
    public bool StartAction()
    {
        DateTime now = DateTime.UtcNow;
        if (!_isDoingAction && ((now - _lastTimeActionFinishedDate).TotalMilliseconds >= _delay))
        {
            _actionStartedDate = now;
            _isDoingAction = true;
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// it resets the timer.
    /// stopping the action does only happen if the action is active.
    /// </summary>
    public void StopAction()
    {
        if(_isDoingAction)
        {
            _isDoingAction = false;
            _lastTimeActionFinishedDate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// A getter for the Action duration in Ms
    /// </summary>
    /// 
    /// <returns>returns the time since the start of the action in ms or -1 if the action wasnt started</returns>
    public float ActionDurationInMs
    {
        get
        {
            if (_isDoingAction)
            {
                return (float)(DateTime.UtcNow - _actionStartedDate).TotalMilliseconds;
            }
            return -1f;
        }
    }
    /// <summary>
    /// A getter for the action. If the action was validly activated it shoul return true
    /// </summary>
    public bool IsDoingAction
    {
        get
        {
            return _isDoingAction;
        }
    }
}