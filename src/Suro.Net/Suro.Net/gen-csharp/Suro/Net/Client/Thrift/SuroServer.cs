/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Suro.Net.Client.Thrift
{
  public partial class SuroServer {
    public interface Iface : SuroService.Iface {
      Result process(TMessageSet messageSet);
      #if SILVERLIGHT
      IAsyncResult Begin_process(AsyncCallback callback, object state, TMessageSet messageSet);
      Result End_process(IAsyncResult asyncResult);
      #endif
    }

    public class Client : SuroService.Client, Iface {
      public Client(TProtocol prot) : this(prot, prot)
      {
      }

      public Client(TProtocol iprot, TProtocol oprot) : base(iprot, oprot)
      {
      }

      
      #if SILVERLIGHT
      public IAsyncResult Begin_process(AsyncCallback callback, object state, TMessageSet messageSet)
      {
        return send_process(callback, state, messageSet);
      }

      public Result End_process(IAsyncResult asyncResult)
      {
        oprot_.Transport.EndFlush(asyncResult);
        return recv_process();
      }

      #endif

      public Result process(TMessageSet messageSet)
      {
        #if !SILVERLIGHT
        send_process(messageSet);
        return recv_process();

        #else
        var asyncResult = Begin_process(null, null, messageSet);
        return End_process(asyncResult);

        #endif
      }
      #if SILVERLIGHT
      public IAsyncResult send_process(AsyncCallback callback, object state, TMessageSet messageSet)
      #else
      public void send_process(TMessageSet messageSet)
      #endif
      {
        oprot_.WriteMessageBegin(new TMessage("process", TMessageType.Call, seqid_));
        process_args args = new process_args();
        args.MessageSet = messageSet;
        args.Write(oprot_);
        oprot_.WriteMessageEnd();
        #if SILVERLIGHT
        return oprot_.Transport.BeginFlush(callback, state);
        #else
        oprot_.Transport.Flush();
        #endif
      }

      public Result recv_process()
      {
        TMessage msg = iprot_.ReadMessageBegin();
        if (msg.Type == TMessageType.Exception) {
          TApplicationException x = TApplicationException.Read(iprot_);
          iprot_.ReadMessageEnd();
          throw x;
        }
        process_result result = new process_result();
        result.Read(iprot_);
        iprot_.ReadMessageEnd();
        if (result.__isset.success) {
          return result.Success;
        }
        throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "process failed: unknown result");
      }

    }
    public class Processor : SuroService.Processor, TProcessor {
      public Processor(Iface iface) : base(iface)
      {
        iface_ = iface;
        processMap_["process"] = process_Process;
      }

      private Iface iface_;

      public new bool Process(TProtocol iprot, TProtocol oprot)
      {
        try
        {
          TMessage msg = iprot.ReadMessageBegin();
          ProcessFunction fn;
          processMap_.TryGetValue(msg.Name, out fn);
          if (fn == null) {
            TProtocolUtil.Skip(iprot, TType.Struct);
            iprot.ReadMessageEnd();
            TApplicationException x = new TApplicationException (TApplicationException.ExceptionType.UnknownMethod, "Invalid method name: '" + msg.Name + "'");
            oprot.WriteMessageBegin(new TMessage(msg.Name, TMessageType.Exception, msg.SeqID));
            x.Write(oprot);
            oprot.WriteMessageEnd();
            oprot.Transport.Flush();
            return true;
          }
          fn(msg.SeqID, iprot, oprot);
        }
        catch (IOException)
        {
          return false;
        }
        return true;
      }

      public void process_Process(int seqid, TProtocol iprot, TProtocol oprot)
      {
        process_args args = new process_args();
        args.Read(iprot);
        iprot.ReadMessageEnd();
        process_result result = new process_result();
        result.Success = iface_.process(args.MessageSet);
        oprot.WriteMessageBegin(new TMessage("process", TMessageType.Reply, seqid)); 
        result.Write(oprot);
        oprot.WriteMessageEnd();
        oprot.Transport.Flush();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class process_args : TBase
    {
      private TMessageSet _messageSet;

      public TMessageSet MessageSet
      {
        get
        {
          return _messageSet;
        }
        set
        {
          __isset.messageSet = true;
          this._messageSet = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool messageSet;
      }

      public process_args() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.Struct) {
                MessageSet = new TMessageSet();
                MessageSet.Read(iprot);
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("process_args");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (MessageSet != null && __isset.messageSet) {
          field.Name = "messageSet";
          field.Type = TType.Struct;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          MessageSet.Write(oprot);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("process_args(");
        sb.Append("MessageSet: ");
        sb.Append(MessageSet== null ? "<null>" : MessageSet.ToString());
        sb.Append(")");
        return sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class process_result : TBase
    {
      private Result _success;

      public Result Success
      {
        get
        {
          return _success;
        }
        set
        {
          __isset.success = true;
          this._success = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool success;
      }

      public process_result() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 0:
              if (field.Type == TType.Struct) {
                Success = new Result();
                Success.Read(iprot);
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("process_result");
        oprot.WriteStructBegin(struc);
        TField field = new TField();

        if (this.__isset.success) {
          if (Success != null) {
            field.Name = "Success";
            field.Type = TType.Struct;
            field.ID = 0;
            oprot.WriteFieldBegin(field);
            Success.Write(oprot);
            oprot.WriteFieldEnd();
          }
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("process_result(");
        sb.Append("Success: ");
        sb.Append(Success== null ? "<null>" : Success.ToString());
        sb.Append(")");
        return sb.ToString();
      }

    }

  }
}
