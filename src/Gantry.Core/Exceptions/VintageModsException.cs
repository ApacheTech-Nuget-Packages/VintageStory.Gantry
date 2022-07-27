#nullable enable
using System;
using System.Runtime.Serialization;

// ReSharper disable CommentTypo

namespace Gantry.Core.Exceptions
{
    /// <summary>
    ///     Acts as a base class for all custom exceptions within the VintageMods domain.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public abstract class VintageModsException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// 	Initialises a new instance of the <see cref="VintageModsException"/> class.
        /// </summary>
        protected VintageModsException()
        {
        }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="VintageModsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected VintageModsException(string message) : base(message)
        {
        }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="VintageModsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that was the cause of the current exception, or a null reference, if no inner exception is provided.</param>
        protected VintageModsException(string message, Exception? inner) : base(message, inner)
        {
        }

        /// <summary>
        /// 	Initialises a new instance of the <see cref="VintageModsException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected VintageModsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
