import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'dart:typed_data';
import 'dart:convert';
import 'package:frontend/Models/OpportunityImg.dart';

class OpportunityImagePicker extends StatefulWidget {
  final List<OpportunityImg> opportunityImgs;
  final ValueChanged<List<OpportunityImg>> onImagesChanged;

  const OpportunityImagePicker({
    Key? key,
    required this.opportunityImgs,
    required this.onImagesChanged,
  }) : super(key: key);

  @override
  _OpportunityImagePickerState createState() => _OpportunityImagePickerState();
}

class _OpportunityImagePickerState extends State<OpportunityImagePicker> {
  void _pickImages() async {
    final List<XFile> pickedFiles = await ImagePicker().pickMultiImage(); // Pick multiple images
    if (pickedFiles != null && pickedFiles.isNotEmpty) {
      if (widget.opportunityImgs.length + pickedFiles.length <= 5) { // Check if we are within the image limit
        final List<String> base64Images = await Future.wait(
          pickedFiles.map((file) async {
            final Uint8List fileBytes = await file.readAsBytes();
            return base64Encode(fileBytes);
          }),
        );
        setState(() {
          List<OpportunityImg> newImages = [];
          for (var image in base64Images) {
            newImages.add(OpportunityImg(imgId: 0, opportunityId: 0, imageBase64: image));
          }
          widget.onImagesChanged([...widget.opportunityImgs, ...newImages]);
        });
      } else {
        // Show error message if more than 5 images are selected
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Não pode adicionar mais de 5 imagens.'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      height: 300, // Increased height for more space
      color: Colors.grey[300],
      child: Column(
        children: [
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.green),
            child: const Text('Escolher Fotos', style: TextStyle(color: Colors.white)),
            onPressed: _pickImages,
          ),
          SizedBox(height: 16.0),
          // Use a SingleChildScrollView to avoid overflow when images are too many
          Expanded(
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: widget.opportunityImgs.map((img) {
                  int index = widget.opportunityImgs.indexOf(img);
                  final Uint8List imageBytes = Uint8List.fromList(
                    base64Decode(img.imageBase64),
                  );
                  return Padding(
                    padding: const EdgeInsets.all(4.0),
                    child: Stack(
                      children: [
                        Image.memory(
                          imageBytes,
                          width: 100,
                          height: 100,
                          fit: BoxFit.cover,
                        ),
                        Positioned(
                          top: 0,
                          right: 0,
                          child: IconButton(
                            icon: Icon(Icons.delete, color: Colors.red),
                            onPressed: () {
                              setState(() {
                                widget.opportunityImgs.removeAt(index); // Remove the image
                                widget.onImagesChanged(widget.opportunityImgs);
                              });
                            },
                          ),
                        ),
                      ],
                    ),
                  );
                }).toList(),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
