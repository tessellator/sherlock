
namespace Sherlock
{
   /// <summary>
   /// A uni-directional communication pipe.
   /// </summary>
   /// <typeparam name="T">
   /// The type of values to pass through the pipe.
   /// </typeparam>
   public interface IPipe<T>
   {
      /// <summary>
      /// Gets a reader for the pipe.
      /// </summary>
      IPipeReader<T> Reader { get; }

      /// <summary>
      /// Gets a writer for the pipe.
      /// </summary>
      IPipeWriter<T> Writer { get; }
   }
}
