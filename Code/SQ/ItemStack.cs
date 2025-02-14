using System;

namespace Sandbox.SQ;

public class ItemStack {
	[ Property ] public Item Item { get; set; }

	[ Property ] public long Count { get; set; }

	[ Property ] public Guid Id { get; set; }
}
