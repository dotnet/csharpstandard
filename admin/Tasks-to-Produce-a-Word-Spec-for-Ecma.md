# Tasks to Produce a Word-Spec for Ecma

While the md version of the spec is very close to the format required for publication, the following tasks need to be performed manually ***after*** the Word version has been generated from the md:

1.	Remove the copyright page.
1.	Page footers: Empty except for a centered page number (Roman in front matter, Arabic for the rest).
1.	Change the TOC format to match the previous editions (by changing the underlying field code).
1.	Page Headers: Recto pages have running chapter/appendix name on RHS, “ECMA-334” on LHS; verso pages the opposite; all bold.
1.	Change the format for each term and definition entry. (*Should have been done to the GitHub repo in pre-v8, so ideally won’t need any editing for V8+.*) 
1.	Add an odd section break prior to the start of each chapter/annex. Also disable auto-page break in h1/appendix1 style. This makes each chapter start on an odd (recto) page number.
1.	Correct Annex clause/subclause numbering (it continues on in Arabic form instead of using a leading alpha). Do this by copying the styles from the (custom modified) previous edition.
1.	The 2 tables in C.4 are in HTML and don’t get converted by the md-to-Word tool. As such, they need to be manually inserted into the final Word draft. Search for “FIXME” placeholders.
