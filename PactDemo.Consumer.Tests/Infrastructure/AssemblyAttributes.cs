// Disable parallel test execution to avoid pact-file write contention.
// Multiple test classes write to the same pact file (same consumer/provider pair),
// so running them serially keeps pact generation deterministic.
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
