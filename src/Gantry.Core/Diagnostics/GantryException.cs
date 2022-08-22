using System;
using System.Runtime.Serialization;

// ReSharper disable CommentTypo

namespace Gantry.Core.Diagnostics
{
    /// <summary>
    ///     Represents errors that occur within the Gantry MDK Framework.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class GantryException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        ///     Initialises a new instance of the <see cref="GantryException"/> class.
        /// </summary>
        public GantryException()
        {
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="GantryException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GantryException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="GantryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public GantryException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="GantryException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected GantryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}