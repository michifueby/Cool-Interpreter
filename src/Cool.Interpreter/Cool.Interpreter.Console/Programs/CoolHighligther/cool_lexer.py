from pathlib import Path
from pygments.lexer import RegexLexer
from pygments.token import *
from pygments import highlight
from pygments.formatters import HtmlFormatter


class CoolLexer(RegexLexer):
    """Lexer for the COOL (Classroom Object Oriented Language) programming language."""
    
    name = 'COOL'
    aliases = ['cool']
    filenames = ['*.cl']

    tokens = {
        'root': [
            # Comments MUST come before operators to avoid matching individual -
            # COOL uses -- for line comments and (* *) for block comments
            (r'--.*?$', Comment.Single),
            (r'\(\*(.|\n)*?\*\)', Comment.Multiline),
            # Keywords (case-insensitive in COOL)
            (r'(?i)\b(class|inherits|let|in|while|loop|pool|then|else|fi|done|new|isvoid|not|case|of|esac|if)\b', Keyword),
            # Boolean literals
            (r'\b(true|false)\b', Keyword.Constant),
            # Type names (start with uppercase)
            (r'\b([A-Z][A-Za-z0-9_]*)\b', Name.Class),
            # Identifiers (start with lowercase or underscore)
            (r'\b([a-z_][A-Za-z0-9_]*)\b', Name.Variable),
            # Integer literals
            (r'\b([0-9]+)\b', Number.Integer),
            # String literals
            (r'"([^"\\]|\\.)*"', String),
            # Assignment operator
            (r'<-', Operator),
            # Comparison operators
            (r'<=|=>|<|>', Operator),
            # Arithmetic operators
            (r'[+\-*/]', Operator),
            # Type annotation and member access
            (r'[:.]', Text),
            # Other operators
            (r'[=~@]', Operator),
            # Delimiters
            (r'[(){}[\],;]', Punctuation),
            # Whitespace
            (r'\s+', Whitespace),
        ]
    }


class CoolHighlighter:
    """Generates syntax-highlighted HTML files for COOL programs."""
    
    # Styling constants
    MONOKAI_COLORS = {
        'background': '#272822',
        'text': '#f8f8f2',
        'comment': '#999999',
    }
    
    @staticmethod
    def get_body_style():
        """Return CSS for body styling."""
        return """
        body {{
            font-family: 'Courier New', monospace;
            margin: 0;
            padding: 20px;
            background: #1e1e1e;
        }}
        .container {{
            max-width: 1200px;
            margin: 0 auto;
        }}
        h1 {{
            font-size: 24px;
            margin-bottom: 30px;
            color: #ffffff;
        }}
        .code-section {{
            border: 1px solid #3c3c3c;
            border-radius: 5px;
            overflow: hidden;
            background: {background};
            padding: 20px;
        }}
        """.format(background=CoolHighlighter.MONOKAI_COLORS['background'])
    
    @staticmethod
    def generate_comment_css():
        """Generate CSS rules for COOL comments."""
        return """
        /* Override comment colors to light grey */
        .source .c1, .source .cm {{ color: {comment} !important; }}
        .source span.c1, .source span.cm {{ color: {comment} !important; }}
        .highlight .c1, .highlight .cm {{ color: {comment} !important; }}
        div.highlight .c1, div.highlight .cm {{ color: {comment} !important; }}
        """.format(comment=CoolHighlighter.MONOKAI_COLORS['comment'])
    
    @staticmethod
    def generate_html(filename: str, code: str) -> str:
        """Generate complete HTML document with syntax-highlighted COOL code."""
        # Create formatter and highlight code
        formatter = HtmlFormatter(
            linenos=True,
            cssclass="source",
            style="monokai",
            full=False
        )
        highlighted = highlight(code, CoolLexer(), formatter)
        css = formatter.get_style_defs('.source')
        
        # Combine all CSS
        all_css = css + CoolHighlighter.generate_comment_css()
        
        # Build HTML document
        html = f"""<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>{filename}</title>
    <style type="text/css">
        {CoolHighlighter.get_body_style()}
        {all_css}
    </style>
</head>
<body>
    <div class="container">
        <h1>{filename}</h1>
        <div class="code-section">
            {highlighted}
        </div>
    </div>
</body>
</html>"""
        return html
    
    @staticmethod
    def process_files(programs_dir: Path, output_dir: Path) -> None:
        """Process all .cl files and generate highlighted HTML files."""
        # Create output directory
        output_dir.mkdir(exist_ok=True)
        
        # Find all .cl files
        cl_files = sorted(programs_dir.glob("*.cl"))
        
        if not cl_files:
            print("No .cl files found in Programs directory")
            return
        
        print(f"Found {len(cl_files)} .cl file(s)\n")
        
        # Process each file
        for cl_file in cl_files:
            print(f"Processing {cl_file.name}...")
            
            # Read source code
            code = cl_file.read_text(encoding='utf-8')
            
            # Generate HTML
            html = CoolHighlighter.generate_html(cl_file.name, code)
            
            # Write output file
            output_file = output_dir / f"{cl_file.stem}.html"
            output_file.write_text(html, encoding='utf-8')
            
            print(f"  ✓ Created {output_file.name}")
        
        print(f"\n✓ All files processed successfully!")
        print(f"✓ Output directory: {output_dir}")


if __name__ == "__main__":
    # Set up directories
    script_dir = Path(__file__).parent
    programs_dir = script_dir.parent
    output_dir = script_dir / "highlighted-files"
    
    # Process files
    CoolHighlighter.process_files(programs_dir, output_dir)
