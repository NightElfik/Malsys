using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malsys.Ast {
	public enum Keyword {
		[StringValue("?unknown?keyword?")]
		Unknown,

		[StringValue("consider")]
		Consider,
		[StringValue("fun")]
		Fun,
		[StringValue("let")]
		Let,
		[StringValue("lsystem")]
		Lsystem,
		[StringValue("nothing")]
		Nothing,
		[StringValue("or")]
		Or,
		[StringValue("return")]
		Return,
		[StringValue("rewrite")]
		Rewrite,
		[StringValue("set")]
		Set,
		[StringValue("to")]
		To,
		[StringValue("weight")]
		Weight,
		[StringValue("with")]
		With,
		[StringValue("where")]
		Where,
		[StringValue("interpret")]
		Interpret,
		[StringValue("as")]
		As,
	}
}
