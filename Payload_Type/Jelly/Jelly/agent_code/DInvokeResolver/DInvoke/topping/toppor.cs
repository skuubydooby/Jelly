using System.Diagnostics;

namespace DInvokeResolver.DInvoke.topping
{
    /// <summary>
    /// Provides static functions for performing topping using a combination of Allocation and Execution components.
    /// </summary>
    /// <author>The Wover (@TheRealWover)</author>
    public static class toppor
    {
        /// <summary>
        /// topp a payload into a target process using a specified allocation and execution technique.
        /// </summary>
        /// <author>The Wover (@TheRealWover)</author>
        /// <param name="Payload"></param>
        /// <param name="AllocationTechnique"></param>
        /// <param name="ExecutionTechnique"></param>
        /// <param name="Process"></param>
        /// <returns></returns>
        public static bool topp(PayloadType Payload, AllocationTechnique AllocationTechnique, ExecutionTechnique ExecutionTechnique, Process Process)
        {
            return ExecutionTechnique.topp(Payload, AllocationTechnique, Process);
        }

        /// <summary>
        /// topp a payload into the current process using a specified allocation and execution technique.
        /// </summary>
        /// <param name="Payload"></param>
        /// <param name="AllocationTechnique"></param>
        /// <param name="ExecutionTechnique"></param>
        /// <returns></returns>
        public static bool topp(PayloadType Payload, AllocationTechnique AllocationTechnique, ExecutionTechnique ExecutionTechnique)
        {
            return ExecutionTechnique.topp(Payload, AllocationTechnique);
        }
    }
}