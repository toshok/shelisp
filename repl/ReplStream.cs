using System;
using System.IO;
using System.Threading;

public class ReplStream : Stream
{
#region Stream overrides
	public override bool CanRead { get { return true; } }
	public override bool CanSeek { get { return false; } }
	public override bool CanWrite { get { return true; } }
	public override long Length { get { return -1; } }
	public override long Position { get { return -1; } set { }}
	public override long Seek (long offset, SeekOrigin origin) { return -1; }
	public override void SetLength (long length) { }

	public override void Flush ()
	{
		// nothing to do here, there's no buffering
	}

	public override int Read (byte[] buffer, int offset, int length)
	{
		// easy case
		if (length == 0)
			return 0;

		int amount_read = 0;

		int current_write_head;

		// this loop should iterate at most twice.
		// the first time might block if there's no
		// data to read.  the second time will only
		// happen after the write thread has unblocked
		// us.
		int amount_to_read;
		while (true) {
			lock (lockobj) {
				current_write_head = write_head;
			}

			if (read_head > current_write_head) {
				// this means that the write head has
				// wrapped around, so the total amount
				// we can read is internal_buffer.Length -
				// read_head + write_head

				amount_to_read = Math.Min (length, internal_buffer.Length - read_head + current_write_head);
			}
			else {
				// amount we can read is write_head -
				// read_head
				amount_to_read = Math.Min (length, current_write_head - read_head);
			}

			if (amount_to_read == 0) {
				dataReady.WaitOne ();
				continue;
			}
			else {
				break; // we have data now, let's get to reading it
			}
		}

		amount_read = amount_to_read;

		if (read_head + amount_to_read > internal_buffer.Length) {
			int first_read_length = internal_buffer.Length - read_head;
			// copy anything up to the end of the internal buffer
			Array.Copy (internal_buffer, read_head, buffer, offset, first_read_length);

			// and then reset things for the second read
			offset += first_read_length;
			lock (lockobj) {
				read_head = 0;
			}
			amount_to_read -= first_read_length;
		}
		Array.Copy (internal_buffer, read_head, buffer, offset, amount_to_read);
		lock (lockobj) {
			read_head += amount_to_read;
		}


		return amount_read;
	}

	public override void Write (byte[] buffer, int offset, int length)
	{
		int current_read_head;
		lock (lockobj) {
			current_read_head = read_head;
		}

		if (length > internal_buffer.Length - write_head + current_read_head)
			throw new Exception ("Houston, we have a problem... the buffer size is insufficient.  do we resize?  I'ma crash");

		if (write_head + length > internal_buffer.Length) {
			int first_write_length = internal_buffer.Length - write_head;

			Array.Copy (buffer, offset, internal_buffer, write_head, first_write_length);
			lock (lockobj) {
				write_head = 0;
			}
			offset += first_write_length;
			length -= first_write_length;
		}

		Array.Copy (buffer, offset, internal_buffer, write_head, length);
		lock (lockobj) {
			write_head += length;
		}
		dataReady.Set();
	}

	int read_head;
	int write_head;

	byte[] internal_buffer = new byte[1024];

	object lockobj = new object();
	AutoResetEvent dataReady = new AutoResetEvent (false);

#endregion

	StreamReader reader = null;
	public StreamReader GetReader()
	{
		return reader ?? (reader = new StreamReader (this));
	}

	StreamWriter writer = null;
	public StreamWriter GetWriter()
	{
		return writer ?? (writer = new StreamWriter (this));
	}
}