namespace UbiCom.Net.Driver
{
    public partial class HSMSDriver
    {
        private void RaiseEventOnSECSConnected(object sender, string ipAddress, int portNo)
        {
            if (this._config.IsAsyncMode == true)
            {
                new SECSConnectedEventHandler(FireOnSECSConnected).BeginInvoke(sender, ipAddress, portNo, null, null);
            }
            else
            {
                new SECSConnectedEventHandler(FireOnSECSConnected)(sender, ipAddress, portNo);
            }
        }

        private void RaiseEventOnSECSDisconnected(object sender, string ipAddress, int portNo)
        {
            if (this._config.IsAsyncMode == true)
            {
                new SECSDisconnectedEventHandler(FireOnSECSDisconnected).BeginInvoke(sender, ipAddress, portNo, null, null);
            }
            else
            {
                new SECSDisconnectedEventHandler(FireOnSECSDisconnected)(sender, ipAddress, portNo);
            }
        }

        private void RaiseEventOnSECSSelected(object sender, string ipAddress, int portNo)
        {
            if (this._config.IsAsyncMode == true)
            {
                new SECSSelectedEventHandler(FireOnSECSSelected).BeginInvoke(sender, ipAddress, portNo, null, null);
            }
            else
            {
                new SECSSelectedEventHandler(FireOnSECSSelected)(sender, ipAddress, portNo);
            }
        }

        private void RaiseEventOnSECSDeselected(object sender, string ipAddress, int portNo)
        {
            if (this._config.IsAsyncMode == true)
            {
                new SECSDeselectedEventHandler(FireOnSECSDeselected).BeginInvoke(sender, ipAddress, portNo, null, null);
            }
            else
            {
                new SECSDeselectedEventHandler(FireOnSECSDeselected)(sender, ipAddress, portNo);
            }
        }

        private void RaiseEventOnTimeout(object sender, Structure.TimeoutType timeoutType)
        {
            if (this._config.IsAsyncMode == true)
            {
                new TimeoutEventHandler(FireOnTimeout).BeginInvoke(sender, timeoutType, null, null);
            }
            else
            {
                new TimeoutEventHandler(FireOnTimeout)(sender, timeoutType);
            }
        }

        private void RaiseEventOnT3Timeout(object sender, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new T3TimeoutEventHandler(FireOnT3Timeout).BeginInvoke(sender, message, null, null);
            }
            else
            {
                new T3TimeoutEventHandler(FireOnT3Timeout)(sender, message);
            }
        }

        private void RaiseEventOnReceivedUnknownMessage(object sender, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new ReceivedUnknownMessageEventHandler(FireOnReceivedUnknownMessage).BeginInvoke(sender, message, null, null);
            }
            else
            {
                new ReceivedUnknownMessageEventHandler(FireOnReceivedUnknownMessage)(sender, message);
            }
        }

        private void RaiseEventOnReceivedInvalidPrimaryMessage(object sender, Structure.MessageValidationError reason, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new ReceivedInvalidPrimaryMessageEventHandler(FireOnReceivedInvalidPrimaryMessage).BeginInvoke(sender, reason, message, null, null);
            }
            else
            {
                new ReceivedInvalidPrimaryMessageEventHandler(FireOnReceivedInvalidPrimaryMessage)(sender, reason, message);
            }
        }

        private void RaiseEventOnReceivedInvalidSecondaryMessage(object sender, Structure.MessageValidationError reason, Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage)
        {
            if (this._config.IsAsyncMode == true)
            {
                new ReceivedInvalidSecondaryMessageEventHandler(FireOnReceivedInvalidSecondaryMessage).BeginInvoke(sender, reason, primaryMessage, secondaryMessage, null, null);
            }
            else
            {
                new ReceivedInvalidSecondaryMessageEventHandler(FireOnReceivedInvalidSecondaryMessage)(sender, reason, primaryMessage, secondaryMessage);
            }
        }

        private void RaiseEventOnSentControlMessage(object sender, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new SentControlMessageEventHandler(FireOnSentControlMessage).BeginInvoke(sender, message, null, null);
            }
            else
            {
                new SentControlMessageEventHandler(FireOnSentControlMessage)(sender, message);
            }
        }

        private void RaiseEventOnSentSECSMessage(object sender, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new SentSECSMessageEventHandler(FireOnSentSECSMessage).BeginInvoke(sender, message, null, null);
            }
            else
            {
                new SentSECSMessageEventHandler(FireOnSentSECSMessage)(sender, message);
            }
        }

        private void RaiseEventOnReceivedControlMessage(object sender, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new ReceivedControlMessageEventHandler(FireOnReceivedControlMessage).BeginInvoke(sender, message, null, null);
            }
            else
            {
                new ReceivedControlMessageEventHandler(FireOnReceivedControlMessage)(sender, message);
            }
        }

        private void RaiseEventOnReceivedPrimaryMessage(object sender, Structure.SECSMessage message)
        {
            if (this._config.IsAsyncMode == true)
            {
                new ReceivedPrimaryMessageEventHandler(FireOnReceivedPrimaryMessage).BeginInvoke(sender, message, null, null);
            }
            else
            {
                new ReceivedPrimaryMessageEventHandler(FireOnReceivedPrimaryMessage)(sender, message);
            }
        }

        private void RaiseEventOnReceivedSecondaryMessage(object sender, Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage)
        {
            if (this._config.IsAsyncMode == true)
            {
                new ReceivedSecondaryMessageEventHandler(FireOnReceivedSecondaryMessage).BeginInvoke(sender, primaryMessage, secondaryMessage, null, null);
            }
            else
            {
                new ReceivedSecondaryMessageEventHandler(FireOnReceivedSecondaryMessage)(sender, primaryMessage, secondaryMessage);
            }
        }

        private void RaiseEventOnSECS1WriteLog(object sender, Utility.Logger.LogLevel logLevel, string logText)
        {
            if (this._config.IsAsyncMode == true)
            {
                new Utility.Logger.LogWriteEventHandler(FireOnSECS1WriteLog).BeginInvoke(sender, logLevel, logText, null, null);
            }
            else
            {
                new Utility.Logger.LogWriteEventHandler(FireOnSECS1WriteLog)(sender, logLevel, logText);
            }
        }

        private void RaiseEventOnSECS2WriteLog(object sender, Utility.Logger.LogLevel logLevel, string logText)
        {
            if (this._config.IsAsyncMode == true)
            {
                new Utility.Logger.LogWriteEventHandler(FireOnSECS2WriteLog).BeginInvoke(sender, logLevel, logText, null, null);
            }
            else
            {
                new Utility.Logger.LogWriteEventHandler(FireOnSECS2WriteLog)(sender, logLevel, logText);
            }
        }

        private void FireOnSECSConnected(object sender, string ipAddress, int portNo)
        {
            this.OnSECSConnected?.Invoke(sender, ipAddress, portNo);
        }

        private void FireOnSECSDisconnected(object sender, string ipAddress, int portNo)
        {
            this.OnSECSDisconnected?.Invoke(sender, ipAddress, portNo);
        }

        private void FireOnSECSSelected(object sender, string ipAddress, int portNo)
        {
            this.OnSECSSelected?.Invoke(sender, ipAddress, portNo);
        }

        private void FireOnSECSDeselected(object sender, string ipAddress, int portNo)
        {
            this.OnSECSDeselected?.Invoke(sender, ipAddress, portNo);
        }

        private void FireOnTimeout(object sender, Structure.TimeoutType timeoutType)
        {
            this.OnTimeout?.Invoke(sender, timeoutType);
        }

        private void FireOnT3Timeout(object sender, Structure.SECSMessage message)
        {
            this.OnT3Timeout?.Invoke(sender, message);
        }

        private void FireOnReceivedUnknownMessage(object sender, Structure.SECSMessage message)
        {
            this.OnReceivedUnknownMessage?.Invoke(sender, message);
        }

        private void FireOnReceivedInvalidPrimaryMessage(object sender, Structure.MessageValidationError reason, Structure.SECSMessage message)
        {
            this.OnReceivedInvalidPrimaryMessage?.Invoke(sender, reason, message);
        }

        private void FireOnReceivedInvalidSecondaryMessage(object sender, Structure.MessageValidationError reason, Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage)
        {
            this.OnReceivedInvalidSecondaryMessage?.Invoke(sender, reason, primaryMessage, secondaryMessage);
        }

        private void FireOnSentControlMessage(object sender, Structure.SECSMessage message)
        {
            this.OnSentControlMessage?.Invoke(sender, message);
        }

        private void FireOnSentSECSMessage(object sender, Structure.SECSMessage message)
        {
            this.OnSentSECSMessage?.Invoke(sender, message);
        }

        private void FireOnReceivedControlMessage(object sender, Structure.SECSMessage message)
        {
            this.OnReceivedControlMessage?.Invoke(sender, message);
        }

        private void FireOnReceivedPrimaryMessage(object sender, Structure.SECSMessage message)
        {
            this.OnReceivedPrimaryMessage?.Invoke(sender, message);
        }

        private void FireOnReceivedSecondaryMessage(object sender, Structure.SECSMessage primaryMessage, Structure.SECSMessage secondaryMessage)
        {
            this.OnReceivedSecondaryMessage?.Invoke(sender, primaryMessage, secondaryMessage);
        }

        void FireOnSECS1WriteLog(object sender, Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnSECS1WriteLog?.Invoke(sender, logLevel, logText);
        }

        void FireOnSECS2WriteLog(object sender, Utility.Logger.LogLevel logLevel, string logText)
        {
            this.OnSECS2WriteLog?.Invoke(sender, logLevel, logText);
        }
    }
}