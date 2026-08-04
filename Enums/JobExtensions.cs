namespace SomberInertia.Enums;

public static class JobExtensions
{
    /// <summary>
    /// True if this unit job shares any flag with <paramref name="allowedJobs"/>.
    /// With <see cref="Job.Any"/> == ~0, universal items allow every non-empty unit job.
    /// </summary>
    public static bool IsAllowedBy(this Job unitJob, Job allowedJobs) =>
        (allowedJobs & unitJob) != 0;
}
